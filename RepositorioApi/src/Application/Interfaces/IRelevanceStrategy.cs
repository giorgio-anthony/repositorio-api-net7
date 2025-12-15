namespace RepositorioApi.Application.Interfaces
{
    public interface IRelevanceStrategy
    {
        /// <summary>
        /// Calcula a relevância do repositório e retorna um score inteiro entre 0 e 100.
        /// </summary>
        int CalculateScore(int stars, int forks, int watchers);
    }
}
