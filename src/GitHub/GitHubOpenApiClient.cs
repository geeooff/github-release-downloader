using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace GitHubReleaseDownloader.GitHub;

internal class GitHubOpenApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private bool _isDisposed;

    public GitHubOpenApiClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.github.com")
        };    

        _httpClient.DefaultRequestHeaders.Add("User-Agent", "GitHub Release Asset Downloader");

        // OpenAPI v3
        _httpClient.DefaultRequestHeaders.Accept.Clear();
        _httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json")
        );
    }

    public Task<GitHubRelease[]?> ListReleases(
        string owner,
        string repo,
        CancellationToken cancellationToken)
        => _httpClient.GetFromJsonAsync(
            $"/repos/{owner}/{repo}/releases",
            GitHubSerializerContext.Default.GitHubReleaseArray,
            cancellationToken);

    public async Task<FileInfo> DownloadAsset(GitHubAsset asset, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(asset.BrowserDownloadUrl))
            throw new ArgumentException("Can't download an asset without a download URL");

        // temp file
        // ref: https://rules.sonarsource.com/csharp/RSPEC-5445/
        var tempFile = new FileInfo(
            Path.Combine(
                Path.GetTempPath(),
                Path.GetRandomFileName()
            )
        );

        // temp file write stream
        using var fileStream = tempFile.OpenWrite();

        // request
        using var request = new HttpRequestMessage(HttpMethod.Get, asset.BrowserDownloadUrl);
        request.Headers.Accept.Clear();

        if (!string.IsNullOrWhiteSpace(asset.ContentType))
        {
            // expecting same content type
            request.Headers.Accept.Add(
                new MediaTypeWithQualityHeaderValue(asset.ContentType)
            );
        }

        try
        {
            // download response
            var response = await _httpClient.SendAsync(request, cancellationToken);
            response.EnsureSuccessStatusCode();

            // download content
            await response.Content.CopyToAsync(fileStream, cancellationToken);

            fileStream.Close();
            tempFile.Refresh();

            return tempFile;
        }
        catch (HttpRequestException)
        {
            fileStream.Close();
            tempFile.Delete();

            Console.Error.WriteLine("Asset Download error");
            throw;
        }
        catch (OperationCanceledException)
        {
            fileStream.Close();
            tempFile.Delete();

            Console.Error.WriteLine("Asset Download was cancelled");
            throw;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (!_isDisposed)
        {
            if (isDisposing)
            {
                _httpClient.Dispose();
            }

            _isDisposed = true;
        }
    }
}
