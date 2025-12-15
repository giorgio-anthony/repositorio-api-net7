using Microsoft.AspNetCore.Mvc;
using RepositorioApi.Application.DTOs;
using RepositorioApi.Application.Interfaces;


namespace RepositorioApi.Api.Controllers;

[ApiController]
[Route("api/github")]
public class GitHubRepositoryController : ControllerBase
{
    private readonly IGitHubRepositoryService _service;
    private readonly ILogger<GitHubRepositoryController> _logger;

    public GitHubRepositoryController(IGitHubRepositoryService service, ILogger<GitHubRepositoryController> logger)
    {
        _service = service;
        _logger = logger;
    }

    /// <summary>
    /// Pesquisa repositórios por nome.
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string repositoryName, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(repositoryName))
            return BadRequest("Query 'repositoryName' is required.");

        var results = await _service.SearchAsync(repositoryName, cancellationToken);
        return Ok(results);
    }

    /// <summary>
    /// Adiciona ou remove do repositório dos favoritos baseado no ID.
    /// </summary>
    [HttpPost("favorites/{id:long}")]
    public async Task<IActionResult> ToggleFavorite([FromRoute] long id, CancellationToken cancellationToken)
    {
        var added = await _service.ToggleFavoriteAsync(id, cancellationToken);
        // Retorna 204 NoContent para operação idempotente; poderia retornar uma mensagem indicando que foi favoritado ou desfavoritado (opicional).
        return NoContent();
    }

    /// <summary>
    /// Lista os repositórios marcados como favoritos.
    /// </summary>
    [HttpGet("favorites")]
    public async Task<IActionResult> Favorites(CancellationToken cancellationToken)
    {
        var results = await _service.ListFavoritesAsync(cancellationToken);
        return Ok(results);
    }

    /// <summary>
    /// Lista os repositórios por relevância. Se a consulta `repositoryName` for fornecida, realiza uma busca; caso contrário, retorna os favoritos ordenados por relevância.
    /// </summary>
    [HttpGet("relevance")]
    public async Task<IActionResult> Relevance([FromQuery] string? repositoryName, CancellationToken cancellationToken)
    {
        var results = await _service.ListByRelevanceAsync(repositoryName, cancellationToken);
        return Ok(results);
    }
}