# RepositorioApi

Aplicação backend em ASP.NET Core (.NET 7) para pesquisa de repositórios públicos do GitHub, marcação de favoritos em memória e ordenação por relevância.

## Visão geral
- Busca repositórios públicos usando a API pública do GitHub.
- Favoritos são mantidos em memória (apenas IDs) durante a execução da aplicação.
- Repositórios podem ser listados ordenados por relevância calculada com base em estrelas, forks e watchers.
- Estrutura em camadas: `Api`, `Application`, `Infrastructure`, `Domain`.

## Principais decisões arquiteturais
- Separação por camadas:
  - `Domain`: modelos de domínio (`GitHubRepository`).
  - `Application`: lógica de negócio, DTOs, serviços e mappers.
  - `Infrastructure`: clientes HTTP para o GitHub e repositório de favoritos (in-memory).
  - `Api`: controllers e configuração do pipeline.

- Dependências invertidas via interfaces (`IGitHubClient`, `IGitHubRepositoryService`, `IFavoritesRepository`, `IMapper`, `IRelevanceStrategy`) para facilitar testes e manutenção.

- Favoritos: armazenamos apenas os IDs no `IFavoritesRepository` (in-memory) — segue SRP e facilita migração para um armazenamento persistente no futuro.

- Mapeamento: centralizado em `IMapper`/`Mapper` para evitar duplicação (DRY).

- Relevância: strategy separada (`IRelevanceStrategy` / `DefaultRelevanceStrategy`) — facilita trocar algoritmo e testá-lo isoladamente.

## Fórmula de relevância
A estratégia padrão utiliza uma combinação logarítmica ponderada para reduzir os vieses de repositórios extremamente populares:

score = 0.6 * log(1 + stars) + 0.3 * log(1 + forks) + 0.1 * log(1 + watchers)

- Peso maior para `stars` (indicador principal de interesse).
- `forks` indica atividade/colaboração e recebe peso secundário.
- `watchers` tem menor peso.
- O `log` reduz impacto de outliers.

(Justificativa comentada no código em `DefaultRelevanceStrategy`.)

## Endpoints principais (API)
- `GET /api/github/search?repositoryName={q}` — busca repositórios por nome/termo.
- `POST /api/github/favorites/{id}` — toggle favorito (favoritar/desfavoritar) pelo `id` do repositório.
- `GET /api/github/favorites` — lista favoritos (cada id é resolvido via `GET /repositories/{id}` do GitHub, em paralelo).
- `GET /api/github/relevance?repositoryName={q}` — lista ordenada por relevância (ou retorna favoritos ordenados se `repositoryName` ausente).

Observação: o controller aceita `CancellationToken` em endpoints para permitir cancelamento de operações longas.

## Como executar
Pré-requisitos: .NET 7 SDK instalado.

1. Restaurar e construir:

```bash
dotnet restore
dotnet build
```

2. Rodar a API:

```bash
dotnet run --project RepositorioApi.csproj
```

A API ficará disponível em `http://localhost:5000` (ou porta configurada). A rota raiz redireciona para Swagger UI.

## Testes
O projeto inclui testes unitários com xUnit e Moq. Para executar os testes:

```bash
dotnet test
```

Para gerar relatórios de cobertura localmente (ex.: coletor XPlat):

```bash
dotnet test --collect:"XPlat Code Coverage"
```

Em seguida use uma ferramenta como `ReportGenerator` para visualizar relatórios em HTML, se desejar.

## Observações importantes / próximos passos
- Polly (retry/circuit-breaker) não foi incluído por padrão — pode ser adicionada facilmente no `Program.cs` e configurada para lidar com rate limits do GitHub.
- Melhorias sugeridas:
  - Adicionar testes unitários adicionais cobrindo falhas do `IGitHubClient` e cancelamento.
  - Implementar persistência de favoritos (SQLite/file) como alternativa ao in-memory.
  - Expor configuração de `maxDegreeOfParallelism` via `appsettings`.

## Decisões de naming / convenções
- Namespaces organizados por camada (`RepositorioApi.Api`, `RepositorioApi.Application`, `RepositorioApi.Infrastructure`, `RepositorioApi.Domain`).
- Interfaces começam com `I` e descrevem contratos de alto nível.

---

Se quiser, eu posso:
- Adicionar ReportGenerator e pipeline de cobertura automatizada (dotnet global tool + script).
- Criar um README mais enxuto em inglês para repositórios públicos.
- Implementar retry com Polly e instruções de configuração de token de acesso ao GitHub para reduzir limites de rate.

— GitHub Copilot
