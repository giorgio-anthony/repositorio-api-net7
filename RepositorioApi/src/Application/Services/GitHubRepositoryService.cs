using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RepositorioApi.Application.DTOs;
using RepositorioApi.Application.Interfaces;
using RepositorioApi.Domain.Models;

namespace RepositorioApi.Application.Services;


public class GitHubRepositoryService : IGitHubRepositoryService
{
    private readonly IGitHubClient _gitHubClient;
    private readonly IFavoritesRepository _favoritesRepo;
    private readonly IRelevanceStrategy _relevanceStrategy;
    private readonly IMapper _mapper;
    private readonly ILogger<GitHubRepositoryService> _logger;

    public GitHubRepositoryService(
        IGitHubClient gitHubClient,
        IFavoritesRepository favoritesRepo,
        IRelevanceStrategy relevanceStrategy,
        IMapper mapper,
        ILogger<GitHubRepositoryService> logger)
    {
        _gitHubClient = gitHubClient;
        _favoritesRepo = favoritesRepo;
        _relevanceStrategy = relevanceStrategy;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Busca repositórios por nome do repositório.
    /// Retorna uma lista contendo com os dados do repositório, uma flag indicando se é favorito e a pontuação de relevância calculado pelo service.
    /// </summary>
    public async Task<List<GitHubRepositoryDto>> SearchAsync(string query, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var repos = await _gitHubClient.SearchRepositoriesAsync(query, cancellationToken);

        var favoriteIds = (await _favoritesRepo.ListAsync()).ToHashSet();

        var dtos = repos
            .Select(r => _mapper.ToRepositoryDto(r, favoriteIds.Contains(r.Id)))
            .Select(dto => dto with
            {
                RelevanceScore = _relevanceStrategy.CalculateScore(dto.Stars, dto.Forks, dto.Watchers)
            })
            .ToList();

        return dtos;
    }

    /// <summary>
    /// Alterna o estado de favorito para o repositório identificado pelo ID. Apenas o ID é armazenado em memória.
    /// Retorna true quando o ID foi adicionado, false quando removido.
    /// </summary>
    public async Task<bool> ToggleFavoriteAsync(long repositoryId, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var exists = await _favoritesRepo.ExistsAsync(repositoryId);

        if (exists)
        {
            await _favoritesRepo.RemoveAsync(repositoryId);
            _logger.LogInformation("Favorito removido {RepositoryId}", repositoryId);
            return false;
        }

        await _favoritesRepo.AddAsync(repositoryId);
        _logger.LogInformation("Favorito adicionado {RepositoryId}", repositoryId);
        return true;
    }

    /// <summary>
    /// Lista os repositórios favoritados. Cada favorito é buscando usando endpoint público do GitHub.
    /// O endpoint público do GitHub não suporta busca em lote, então as requisições são feitas em paralelo com controle de concorrência.
    /// </summary>
    public async Task<List<GitHubRepositoryDto>> ListFavoritesAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var ids = await _favoritesRepo.ListAsync();
        if (ids == null || ids.Count == 0)
            return new List<GitHubRepositoryDto>();

        const int maxDegreeOfParallelism = 8;
        using var semaphore = new SemaphoreSlim(maxDegreeOfParallelism);

        var tasks = ids.Select(async id =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var repo = await _gitHubClient.GetRepositoryByIdAsync(id, cancellationToken);
                if (repo == null)
                {
                    _logger.LogWarning("ID de repositório {Id} não encontrado no GitHub", id);
                    return null;
                }

                var dto = _mapper.ToRepositoryDto(repo, true);
                dto.RelevanceScore = _relevanceStrategy.CalculateScore(dto.Stars, dto.Forks, dto.Watchers);
                return dto;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar repositório {Id}", id);
                return null;
            }
            finally
            {
                semaphore.Release();
            }
        }).ToList();

        var results = await Task.WhenAll(tasks);
        return results.Where(r => r != null).Select(r => r!).ToList();
    }

    /// <summary>
    /// Retorna uma lista ordenada por relevância. Se for fornecido o nome do repositório, realiza a busca e ordena os resultados;
    /// caso contrário, retorna os favoritos ordenados por relevância.
    /// </summary>
    public async Task<List<GitHubRepositoryDto>> ListByRelevanceAsync(string? query, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(query))
            return (await ListFavoritesAsync(cancellationToken)).OrderByDescending(f => f.RelevanceScore).ToList();

        var searchResults = await SearchAsync(query, cancellationToken);
        return searchResults.OrderByDescending(f => f.RelevanceScore).ToList();
    }

    /// <summary>
    /// Obtém detalhes de repositório por ID.
    /// </summary>
    public async Task<GitHubRepositoryDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var repo = await _gitHubClient.GetRepositoryByIdAsync(id, cancellationToken);
        if (repo == null) return null;

        var favorites = await _favoritesRepo.ListAsync();
        var isFav = favorites.Any(f => f == repo.Id);

        var dto = _mapper.ToRepositoryDto(repo, isFav);
        dto.RelevanceScore = _relevanceStrategy.CalculateScore(dto.Stars, dto.Forks, dto.Watchers);
        return dto;
    }

}
