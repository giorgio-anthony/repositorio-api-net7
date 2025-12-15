using RepositorioApi.Application.DTOs;
using RepositorioApi.Application.Interfaces;
using RepositorioApi.Domain.Models;

namespace RepositorioApi.Application.Mappers;

/// <summary>
/// Implementação para mapeamento entre modelos de domínio e DTOs da aplicação.
/// Centraliza a lógica de conversão para facilitar manutenção.
/// </summary>
public class Mapper : IMapper
{
    public GitHubRepositoryDto ToRepositoryDto(GitHubRepository repo, bool isFavorite)
    {
        return new GitHubRepositoryDto
        {
            Id = repo.Id,
            Name = repo.Name,
            FullName = repo.FullName,
            HtmlUrl = repo.HtmlUrl,
            Description = repo.Description,
            Stars = repo.StargazersCount,
            Forks = repo.ForksCount,
            Watchers = repo.WatchersCount,
            IsFavorite = isFavorite
        };
    }

    public FavoriteRepositoryDto ToFavoriteDto(GitHubRepository repo)
    {
        return new FavoriteRepositoryDto
        {
            Id = repo.Id,
            Name = repo.Name,
            FullName = repo.FullName,
            HtmlUrl = repo.HtmlUrl,
            Description = repo.Description,
            Stars = repo.StargazersCount,
            Forks = repo.ForksCount,
            Watchers = repo.WatchersCount
        };
    }
}
