using RepositorioApi.Application.Interfaces;

namespace RepositorioApi.Application.Utils;

public class DefaultRelevanceStrategy : IRelevanceStrategy
{
    // Pesos de cada métrica
    private const double StarsWeight = 0.6;
    private const double ForksWeight = 0.3;
    private const double WatchersWeight = 0.1;

    // Maior valor bruto esperado antes de normalizar o score para 0–100
    // log10(200000) ≈ 5.3 → score bruto máximo ~ 5.3
    // O limite de 200000 foi escolhido baseado nos maiores repositórios do GitHub.
    // Os maiores repositórios públicos atualmente têm em torno de 200k estrelas.
    // E a quantidade de watchers e Forks é proporcionalmente menor.
    // A normalização ajuda a manter o score final dentro de 0–100.
    // A escolha do score de 0-100 foi feita para facilitar a compreensão do usuário final.
    private const double MaxRawScore = 5.3;

    public int CalculateScore(int stars, int forks, int watchers)
    {
        // Score bruto usando log10 para conter valores muito grandes
        double rawScore =
            StarsWeight * Math.Log10(1 + stars) +
            ForksWeight * Math.Log10(1 + forks) +
            WatchersWeight * Math.Log10(1 + watchers);

        // Normaliza para 0–100
        double normalized = (rawScore / MaxRawScore) * 100;

        // Limita e converte para inteiro
        return (int)Math.Clamp(Math.Round(normalized), 0, 100);
    }
}
