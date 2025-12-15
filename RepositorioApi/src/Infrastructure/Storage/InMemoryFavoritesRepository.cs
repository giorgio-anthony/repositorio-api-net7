using RepositorioApi.Application.Interfaces;

namespace RepositorioApi.Infrastructure.Storage;

/// <summary>
/// Repositório de favoritos in-memory. Mantém IDs dos repositórios favoritados.
/// - Projetado para uso em tempo de execução (volátil) sem persistência.
/// - Implementa bloqueio simples (lock) para operações thread-safe 
/// (evitando manipulação simultânea da lista de favoritos causando possíveis inconsistências).
/// </summary>
public class InMemoryFavoritesRepository : IFavoritesRepository
{
    private readonly List<long> _favorites = new();
    private readonly object _lock = new();

    /// <summary>
    /// Adiciona um id à lista de favoritos (não adiciona duplicatas).
    /// </summary>
    public Task AddAsync(long id)
    {
        lock (_lock)
        {
            if (!_favorites.Contains(id))
                _favorites.Add(id);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Remove um id da lista de favoritos.
    /// </summary>
    public Task RemoveAsync(long id)
    {
        lock (_lock)
        {
            _favorites.RemoveAll(f => f == id);
        }
        return Task.CompletedTask;
    }

    /// <summary>
    /// Verifica se um id está presente nos favoritos.
    /// </summary>
    public Task<bool> ExistsAsync(long id)
    {
        lock (_lock)
        {
            return Task.FromResult(_favorites.Contains(id));
        }
    }

    /// <summary>
    /// Retorna a cópia atual da lista de ids favoritados.
    /// </summary>
    public Task<List<long>> ListAsync()
    {
        lock (_lock)
        {
            return Task.FromResult(_favorites.ToList());
        }
    }
}
