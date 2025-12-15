using RepositorioApi.Application.DTOs;
using System.Threading;

namespace RepositorioApi.Application.Interfaces;


public interface IGitHubRepositoryService
{
    /// <summary>
    /// Busca repositórios por nome/parte do nome.
    /// </summary>
    Task<List<GitHubRepositoryDto>> SearchAsync(string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Adiciona ou remove o ID do repositório da lista de favoritos. Retorna true se o repositório foi adicionado, false se removido.
    /// </summary>
    Task<bool> ToggleFavoriteAsync(long repositoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista repositórios marcados como favoritos.
    /// </summary>
    Task<List<GitHubRepositoryDto>> ListFavoritesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Lista repositórios ordenados por relevância.
    /// </summary>
    Task<List<GitHubRepositoryDto>> ListByRelevanceAsync(string? query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém detalhes do repositório pelo id do GitHub.
    /// </summary>
    Task<GitHubRepositoryDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);

}
