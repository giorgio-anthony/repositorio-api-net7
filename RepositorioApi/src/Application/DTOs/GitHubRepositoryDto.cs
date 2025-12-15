namespace RepositorioApi.Application.DTOs;

public record GitHubRepositoryDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string HtmlUrl { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Stars { get; init; }
    public int Forks { get; init; }
    public int Watchers { get; init; }
    public double RelevanceScore { get; set; }   // Calculado pela Application/Services
    public bool IsFavorite { get; set; }
}
