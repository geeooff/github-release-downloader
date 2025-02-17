using GitHubReleaseDownloader.GitHub;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace GitHubReleaseDownloader;

internal static class ProgramHandler
{
    public static async Task<int> Execute(
        string owner,
        string repo,
        FileInfo output,
        bool force,
        bool preRelease,
        bool createBackup,
        CancellationToken cancellationToken)
    {
        using var client = new GitHubOpenApiClient();

        // latest release
        var releases = await client.ListReleases(owner, repo, cancellationToken);
        var latestRelease = GetLatestRelease(releases, preRelease);

        // check release
        if (latestRelease != null)
        {
            var releaseDescription = latestRelease.GetDescription();
            Console.Out.WriteLine($"Latest release of {owner}/{repo}: {releaseDescription}");
        }
        else
        {
            Console.Error.WriteLine($"No release found for {owner}/{repo}");
            return 1;
        }

        // latest asset
        var latestAsset = GetAsset(latestRelease, output);

        // check asset
        if (latestAsset != null)
        {
            var assetDescription = latestAsset.GetDescription();
            Console.Out.WriteLine($"Latest asset: {assetDescription}");
        }
        else
        {
            Console.Error.WriteLine($"No asset found for this release");
            return 2;
        }

        // check if download should be done
        if (!(force || ShouldDownload(latestAsset, output)))
        {
            Console.Error.WriteLine("Local asset is up to date");
            return 0;
        }

        // asset download
        FileInfo assetFile = await client.DownloadAsset(latestAsset, cancellationToken);

        // saving to existing file
        string outputPath = output.FullName;
        if (output.Exists)
        {
            if (createBackup)
            {
                Console.Out.WriteLine("Backuping and replacing existing file...");
                output.MoveTo($"{outputPath}.bak", true);
            }
            else
            {
                Console.Out.WriteLine("Replacing existing file...");
            }
        }
        else
        {
            // moving temp file to final destination
            Console.Out.WriteLine("Moving downloaded file...");
        }

        assetFile.MoveTo(outputPath);

        // date sync
        var assetDate = latestAsset.GetDate()?.ToUniversalTime();
        if (assetDate.HasValue)
        {
            Console.Out.WriteLine("Synching downloaded file date...");
            assetFile.LastWriteTimeUtc = assetDate.Value;
        }

        Console.Out.WriteLine("Asset saved successfully");

        return 0;
    }

    internal static GitHubRelease? GetLatestRelease(GitHubRelease[]? releases, bool preRelease)
    {
        if (releases == null || releases.Length == 0)
            return null;

        if (!preRelease)
        {
            // filtering pre-releases
            releases = [.. releases.Where(r => r.PreRelease == false)];
        }

        // sorting releases by date
        releases = [.. releases.OrderByDescending(r => r.GetDate()) ];

        return releases.FirstOrDefault();
    }

    internal static GitHubAsset? GetAsset(GitHubRelease? release, FileInfo output)
    {
        if (release?.Assets == null || release.Assets.Length == 0)
            return null;

        var assetName = output.Name;
        var assets = release.Assets.Where(asset => asset.Name == assetName).ToArray();

        if (assets.Length > 1)
        {
            Console.Error.WriteLine($"Found several assets with name \"{assetName}\" in release \"{release.Name}\"");
            return null;
        }
        else
            return assets.SingleOrDefault();
    }

    internal static bool ShouldDownload(GitHubAsset asset, FileInfo output)
    {
        // asset date
        var assetDate = asset.GetDate()?.ToUniversalTime();

        // won't download asset without date
        if (!assetDate.HasValue)
            return false;

        // should download if no local file
        // or local file outdated
        return !output.Exists || output.LastWriteTimeUtc < assetDate;
    }
}
