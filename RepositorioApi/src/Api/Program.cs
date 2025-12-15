using Microsoft.OpenApi.Models;
using RepositorioApi.Application.Interfaces;
using RepositorioApi.Application.Mappers;
using RepositorioApi.Application.Services;
using RepositorioApi.Application.Utils;
using RepositorioApi.Infrastructure.GitHub;
using RepositorioApi.Infrastructure.Storage;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "RepositorioApi", Version = "v1" });
});

// Configura HttpClient para IGitHubClient com headers básicos
builder.Services.AddHttpClient<IGitHubClient, GitHubClient>(client =>
{
    client.BaseAddress = new Uri("https://api.github.com/");
    client.DefaultRequestHeaders.UserAgent.ParseAdd("RepositorioApiClient/1.0");
    client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github.v3+json");
});

// Repositório de favoritos: singleton (in-memory) para manter os IDs durante a execução
builder.Services.AddSingleton<IFavoritesRepository, InMemoryFavoritesRepository>();
// Estratégia de relevância e Mapper é stateless
builder.Services.AddSingleton<IRelevanceStrategy, DefaultRelevanceStrategy>();
builder.Services.AddSingleton<IMapper, Mapper>();

// Serviços da aplicação
builder.Services.AddScoped<IGitHubRepositoryService, GitHubRepositoryService>();

var app = builder.Build();

// Redireciona raiz para Swagger UI ao rodar a aplicação
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger");
        return;
    }

    await next();
});

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
app.MapControllers();
app.Run();