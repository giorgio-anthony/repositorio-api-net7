using RepositorioApi.Application.Interfaces;
using RepositorioApi.Domain.Models;

namespace RepositorioApi.Infrastructure.GitHub;

/// <summary>
/// Cliente HTTP para consumir endpoints públicos do GitHub.
/// - Implementa apenas os métodos necessários pelo serviço de aplicação.
/// - Retorna objetos do domínio `GitHubRepository` desserializados do JSON da API.
/// - Em caso de falhas ou resposta inválida, os métodos retornam valores neutros (lista vazia ou null).
/// </summary>
public class GitHubClient : IGitHubClient
{
    private readonly HttpClient _http;

    public GitHubClient(HttpClient http)
    {
        _http = http;
        _http.BaseAddress = new Uri("https://api.github.com/"); /// Poderia ser configurado na .env ou appsettings.
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("API-Test-App");
    }

    /// <summary>
    /// Busca repositórios via endpoint de pesquisa do GitHub.
    /// Retorna lista vazia em caso de erro. Aceita CancellationToken para cancelar a requisição.
    /// </summary>
    public async Task<List<GitHubRepository>> SearchRepositoriesAsync(string query, CancellationToken cancellationToken = default)
    {
        var url = $"search/repositories?q={Uri.EscapeDataString(query)}";

        try
        {
            var response = await _http.GetFromJsonAsync<GitHubSearchResponse>(url, cancellationToken);
            return response?.Items ?? new List<GitHubRepository>();
        }
        catch
        {
            return new List<GitHubRepository>();
        }
    }

    /// <summary>
    /// Obtém um repositório pelo ID numérico usando /repositories/{id}.
    /// Retorna null em caso de erro ou se o repositório não existir.
    /// </summary>
    public async Task<GitHubRepository?> GetRepositoryByIdAsync(long id, CancellationToken cancellationToken = default)
    {
        try
        {
            var url = $"repositories/{id}";
            var response = await _http.GetFromJsonAsync<GitHubRepository>(url, cancellationToken);
            return response;
        }
        catch
        {
            return null;
        }
    }

    private class GitHubSearchResponse
    {
        // Estrutura interna usada para desserializar a resposta de busca do GitHub
        public List<GitHubRepository> Items { get; set; } = new();
    }
}
