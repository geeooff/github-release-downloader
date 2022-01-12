using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace GitHubReleaseDownloader.GitHub;

internal class GitHubRelease
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("draft")]
    public bool? Draft { get; set; }

    [JsonPropertyName("prerelease")]
    public bool? PreRelease { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime? CreatedAt { get; set; }

    [JsonPropertyName("published_at")]
    public DateTime? PublishedAt { get; set; }

    [JsonPropertyName("assets")]
    public GitHubAsset[]? Assets { get; set; }
}
