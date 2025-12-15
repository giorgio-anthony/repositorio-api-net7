using RepositorioApi.Application.Utils;
using Xunit;

namespace RepositorioApi.Tests.ApplicationTests;

public class DefaultRelevanceStrategyTests
{
    private readonly DefaultRelevanceStrategy _strategy = new();

    [Fact(DisplayName = "CalculateScore: Valida se o score é 0 quando todas as métricas estão zeradas.")]
    public void CalculateScore_ShouldReturnZero_WhenAllValuesAreZero()
    {
        int score = _strategy.CalculateScore(0, 0, 0);

        Assert.Equal(0, score);
    }

    [Fact(DisplayName = "CalculateScore: Valida se o método está considerando o peso das stars.")]
    public void CalculateScore_ShouldIncrease_WhenStarsIncrease()
    {
        int low = _strategy.CalculateScore(10, 0, 0);
        int high = _strategy.CalculateScore(1000, 0, 0);

        Assert.True(high > low);
    }

    [Fact(DisplayName = "CalculateScore: Valida se o método considerando o peso das forks.")]
    public void CalculateScore_ShouldConsiderForksWeight()
    {
        int scoreWithoutForks = _strategy.CalculateScore(100, 0, 0);
        int scoreWithForks = _strategy.CalculateScore(100, 500, 0);

        Assert.True(scoreWithForks > scoreWithoutForks);
    }

    [Fact(DisplayName = "CalculateScore: Valida se o método considerando o peso dos watchers.")]
    public void CalculateScore_ShouldConsiderWatchersWeight()
    {
        int scoreWithoutWatchers = _strategy.CalculateScore(100, 0, 0);
        int scoreWithWatchers = _strategy.CalculateScore(100, 0, 500);

        Assert.True(scoreWithWatchers > scoreWithoutWatchers);
    }

    [Fact(DisplayName = "CalculateScore: Valida se o método está excedendo o valor máximo (100).")]
    public void CalculateScore_ShouldNeverExceed100()
    {
        int score = _strategy.CalculateScore(1_000_000, 1_000_000, 1_000_000);

        Assert.Equal(100, score);
    }

    [Fact(DisplayName = "CalculateScore: Valida se está retornando número inteiro.")]
    public void CalculateScore_ShouldReturnInteger()
    {
        int score = _strategy.CalculateScore(1234, 567, 89);

        Assert.IsType<int>(score);
    }

    [Fact(DisplayName = "CalculateScore: Valida se o score está aplicando os pesos nas métricas corretamente.")]
    public void CalculateScore_ShouldBeMonotonicIncreasing()
    {
        int scoreA = _strategy.CalculateScore(50, 30, 10);
        int scoreB = _strategy.CalculateScore(100, 60, 20);

        Assert.True(scoreB > scoreA);
    }

    [Fact(DisplayName = "CalculateScore: Valida o output de um caso típico de repositório.")]
    public void CalculateScore_ShouldNormalizeCorrectly()
    {
        int score = _strategy.CalculateScore(1000, 500, 100);

        Assert.InRange(score, 40, 70);
    }
}
