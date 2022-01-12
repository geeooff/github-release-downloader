using System.Text;

namespace GitHubReleaseDownloader.GitHub;

internal static class GitHubExtensions
{
    internal static DateTime? GetDate(this GitHubRelease release) => release?.PublishedAt;

    internal static DateTime? GetDate(this GitHubAsset asset) => asset?.UpdatedAt ?? asset?.CreatedAt;

    internal static string? GetDescription(this GitHubAsset asset)
    {
        if (asset == null)
            return null;

        StringBuilder description = new(asset.Name);

        if (asset.Size > 0)
        {
            description.Append($", {asset.Size} bytes");
        }

        if (asset.UpdatedAt.HasValue)
        {
            description.Append($", updated at {asset.UpdatedAt}");
        }
        else if (asset.CreatedAt.HasValue)
        {
            description.Append($", created at {asset.CreatedAt}");
        }

        return description.ToString();
    }

    internal static string? GetDescription(this GitHubRelease release)
    {
        if (release == null)
            return null;

        StringBuilder description = new(release.Name);

        if (release.PreRelease ?? false)
        {
            description.Append(" (pre-release)");
        }

        if (release.Draft ?? false)
        {
            description.Append(" (draft)");
        }

        if (release.PublishedAt.HasValue)
        {
            description.Append($", published at {release.PublishedAt}");
        }
        else if (release.CreatedAt.HasValue)
        {
            description.Append($", created at {release.CreatedAt}");
        }

        return description.ToString();
    }
}
