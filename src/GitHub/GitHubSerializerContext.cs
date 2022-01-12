using System.Text.Json;
using System.Text.Json.Serialization;

namespace GitHubReleaseDownloader.GitHub;

[JsonSourceGenerationOptions(JsonSerializerDefaults.Web)]
[JsonSerializable(typeof(GitHubAsset))]
[JsonSerializable(typeof(GitHubRelease))]
[JsonSerializable(typeof(GitHubRelease[]))]
internal partial class GitHubSerializerContext : JsonSerializerContext
{

}