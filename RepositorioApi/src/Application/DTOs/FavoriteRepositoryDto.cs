namespace RepositorioApi.Application.DTOs;

/// <summary>
/// DTO que representa um repositório favorito (retornado pelo endpoint de favoritos).
/// Mantido separado de GitHubRepositoryDto, pois não inclui a propriedade IsFavorite.
/// </summary>
public record FavoriteRepositoryDto
{
    public long Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string HtmlUrl { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int Stars { get; init; }
    public int Forks { get; init; }
    public int Watchers { get; init; }
}
