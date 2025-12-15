using RepositorioApi.Application.DTOs;

namespace RepositorioApi.Application.Interfaces;

/// <summary>
/// Repositório de favoritos (contrato). Responsável por armazenar/consultar ids de repositórios marcados como favoritos.
/// Implementações podem ser in-memory (volátil) ou persistentes; a aplicação depende apenas deste contrato.
/// </summary>
public interface IFavoritesRepository
{
    /// <summary>
    /// Adiciona o ID do repositório à lista de favoritos.
    /// </summary>
    Task AddAsync(long id);

    /// <summary>
    /// Remove o ID do repositório da lista de favoritos.
    /// </summary>
    Task RemoveAsync(long id);

    /// <summary>
    /// Verifica se o ID já está presente na lista de favoritos.
    /// </summary>
    Task<bool> ExistsAsync(long id);

    /// <summary>
    /// Retorna a lista atual de IDs favoritados.
    /// </summary>
    Task<List<long>> ListAsync();
}
