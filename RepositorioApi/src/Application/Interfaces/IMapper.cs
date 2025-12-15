using RepositorioApi.Application.DTOs;
using RepositorioApi.Domain.Models;

namespace RepositorioApi.Application.Interfaces;

/// <summary>
/// Mapper central para converter modelos de domínio em DTOs da aplicação.
/// Centraliza os mapeamentos para seguir DRY e responsabilidade única.
/// </summary>
public interface IMapper
{
    GitHubRepositoryDto ToRepositoryDto(GitHubRepository repo, bool isFavorite);
    FavoriteRepositoryDto ToFavoriteDto(GitHubRepository repo);
}
