using System.Text.Json.Serialization;

namespace RepositorioApi.Domain.Models;

public record GitHubRepository
{
    [JsonPropertyName("id")]
    public long Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("full_name")]
    public string FullName { get; init; } = string.Empty;

    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("stargazers_count")]
    public int StargazersCount { get; init; }

    [JsonPropertyName("forks_count")]
    public int ForksCount { get; init; }

    [JsonPropertyName("watchers_count")]
    public int WatchersCount { get; init; }
}
