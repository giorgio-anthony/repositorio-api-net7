using Moq;
using RepositorioApi.Application.Interfaces;
using RepositorioApi.Application.Mappers;
using RepositorioApi.Application.Services;
using RepositorioApi.Domain.Models;
using Xunit;

namespace RepositorioApi.Tests.ApplicationTests;

public class GitHubRepositoryServiceTests
{
    private GitHubRepositoryService CreateService(
        Mock<IGitHubClient>? gitHubClient = null,
        Mock<IFavoritesRepository>? favoritesRepo = null,
        Mock<IRelevanceStrategy>? relevanceStrategy = null,
        IMapper? mapper = null)
    {
        gitHubClient ??= new Mock<IGitHubClient>();
        favoritesRepo ??= new Mock<IFavoritesRepository>();
        relevanceStrategy ??= new Mock<IRelevanceStrategy>();
        mapper ??= new Mapper();

        var logger = new Mock<ILogger<GitHubRepositoryService>>().Object;

        return new GitHubRepositoryService(
            gitHubClient.Object,
            favoritesRepo.Object,
            relevanceStrategy.Object,
            mapper,
            logger);
    }

    [Fact(DisplayName = "SearchAsync: Busca de repositórios por nome/parte do nome com sucesso")]
    public async Task SearchAsync_OrdersByRelevance()
    {
        var gitHubClient = new Mock<IGitHubClient>();
        var favoritesRepo = new Mock<IFavoritesRepository>();
        var relevanceStrategy = new Mock<IRelevanceStrategy>();
        var mapper = new Mapper();

        // Criação dos repositórios simulados
        var repoA = new GitHubRepository { Id = 1, Name = "a", FullName = "a/a", HtmlUrl = "u", Description = "d", StargazersCount = 10, ForksCount = 0, WatchersCount = 0 };
        var repoB = new GitHubRepository { Id = 2, Name = "b", FullName = "b/b", HtmlUrl = "u2", Description = "d2", StargazersCount = 100, ForksCount = 0, WatchersCount = 0 };

        gitHubClient.Setup(x => x.SearchRepositoriesAsync("q", It.IsAny<CancellationToken>())).ReturnsAsync(new List<GitHubRepository> { repoA, repoB });
        favoritesRepo.Setup(x => x.ListAsync()).ReturnsAsync(new List<long>());

        // Estratégia de relevância: simples (retorna apenas stars)
        relevanceStrategy.Setup(x => x.CalculateScore(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns<int,int,int>((s, f, w) => s);

        var service = CreateService(gitHubClient, favoritesRepo, relevanceStrategy, mapper);

        var result = await service.SearchAsync("q");

        Assert.Equal(2, result.Count);
    }

    [Fact(DisplayName = "ToggleFavoriteAsync: Adiciona ID do repositória à lista de favoritos com sucesso")]
    public async Task ToggleFavoriteAsync_Adds_When_Not_Exists()
    {
        var gitHubClient = new Mock<IGitHubClient>();
        var favoritesRepo = new Mock<IFavoritesRepository>();
        favoritesRepo.Setup(x => x.ExistsAsync(10)).ReturnsAsync(false);

        var service = CreateService(gitHubClient, favoritesRepo, new Mock<IRelevanceStrategy>(), new Mapper());

        var added = await service.ToggleFavoriteAsync(10);

        Assert.True(added);
        favoritesRepo.Verify(x => x.AddAsync(10), Times.Once);
    }

    [Fact(DisplayName = "ToggleFavoriteAsync: Remove ID do repositória da lista de favoritos com sucesso")]
    public async Task ToggleFavoriteAsync_Removes_When_Exists()
    {
        var gitHubClient = new Mock<IGitHubClient>();
        var favoritesRepo = new Mock<IFavoritesRepository>();
        favoritesRepo.Setup(x => x.ExistsAsync(11)).ReturnsAsync(true);

        var service = CreateService(gitHubClient, favoritesRepo, new Mock<IRelevanceStrategy>(), new Mapper());

        var added = await service.ToggleFavoriteAsync(11);

        Assert.False(added);
        favoritesRepo.Verify(x => x.RemoveAsync(11), Times.Once);
    }

    [Fact(DisplayName = "ListFavoritesAsync: Busca repositórios favoritados com sucesso")]
    public async Task ListFavoritesAsync_FetchesRepositories()
    {
        var gitHubClient = new Mock<IGitHubClient>();
        var favoritesRepo = new Mock<IFavoritesRepository>();
        var relevanceStrategy = new Mock<IRelevanceStrategy>();
        var mapper = new Mapper();

        favoritesRepo.Setup(x => x.ListAsync()).ReturnsAsync(new List<long> { 100, 200 });

        var repo1 = new GitHubRepository { Id = 100, Name = "r1", FullName = "r/r1", HtmlUrl = "u1", Description = "d1", StargazersCount = 5, ForksCount = 1, WatchersCount = 0 };
        var repo2 = new GitHubRepository { Id = 200, Name = "r2", FullName = "r/r2", HtmlUrl = "u2", Description = "d2", StargazersCount = 15, ForksCount = 2, WatchersCount = 0 };

        gitHubClient.Setup(x => x.GetRepositoryByIdAsync(100, It.IsAny<CancellationToken>())).ReturnsAsync(repo1);
        gitHubClient.Setup(x => x.GetRepositoryByIdAsync(200, It.IsAny<CancellationToken>())).ReturnsAsync(repo2);

        relevanceStrategy.Setup(x => x.CalculateScore(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns<int,int,int>((s,f,w) => s);

        var service = CreateService(gitHubClient, favoritesRepo, relevanceStrategy, mapper);

        var results = await service.ListFavoritesAsync();

        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.Id == 100);
        Assert.Contains(results, r => r.Id == 200);
    }

    [Fact(DisplayName = "GetByIdAsync: Valida se o repositório foi favoritado com sucesso")]
    public async Task GetByIdAsync_SetsIsFavoriteTrue_WhenInFavorites()
    {
        var gitHubClient = new Mock<IGitHubClient>();
        var favoritesRepo = new Mock<IFavoritesRepository>();
        var relevanceStrategy = new Mock<IRelevanceStrategy>();
        var mapper = new Mapper();

        var repo = new GitHubRepository { Id = 55, Name = "r", FullName = "r/r", HtmlUrl = "u", Description = "d", StargazersCount = 1, ForksCount = 0, WatchersCount = 0 };
        gitHubClient.Setup(x => x.GetRepositoryByIdAsync(55, It.IsAny<CancellationToken>())).ReturnsAsync(repo);
        favoritesRepo.Setup(x => x.ListAsync()).ReturnsAsync(new List<long> { 55 });
        relevanceStrategy.Setup(x => x.CalculateScore(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1);

        var service = CreateService(gitHubClient, favoritesRepo, relevanceStrategy, mapper);

        var dto = await service.GetByIdAsync(55);

        Assert.NotNull(dto);
        Assert.True(dto!.IsFavorite);
    }

    [Fact(DisplayName = "ListByRelevanceAsync: Faz busca ordenada por relevância com sucesso")]
    public async Task ListByRelevanceAsync_WithQuery_UsesSearch()
    {
        var gitHubClient = new Mock<IGitHubClient>();
        var favoritesRepo = new Mock<IFavoritesRepository>();
        var relevanceStrategy = new Mock<IRelevanceStrategy>();
        var mapper = new Mapper();

        var repo = new GitHubRepository { Id = 99, Name = "repo", FullName = "r/repo", HtmlUrl = "u", Description = "d", StargazersCount = 2, ForksCount = 0, WatchersCount = 0 };
        gitHubClient.Setup(x => x.SearchRepositoriesAsync("term", It.IsAny<CancellationToken>())).ReturnsAsync(new List<GitHubRepository> { repo });
        favoritesRepo.Setup(x => x.ListAsync()).ReturnsAsync(new List<long>());
        relevanceStrategy.Setup(x => x.CalculateScore(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(2);

        var service = CreateService(gitHubClient, favoritesRepo, relevanceStrategy, mapper);

        var results = await service.ListByRelevanceAsync("term");

        Assert.Single(results);
        Assert.Equal(99, results[0].Id);
    }
}
