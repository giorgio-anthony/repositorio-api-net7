using RepositorioApi.Domain.Models;

namespace RepositorioApi.Application.Interfaces;


public interface IGitHubClient
{
    /// <summary>
    /// Busca repositórios por nome/parte do nome.
    /// </summary>
    Task<List<GitHubRepository>> SearchRepositoriesAsync(string query, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um repositório pelo ID do repositório.
    /// </summary>
    Task<GitHubRepository?> GetRepositoryByIdAsync(long id, CancellationToken cancellationToken = default);
}
