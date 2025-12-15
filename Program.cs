using System.Text.Json;
using System.Net.Http.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRepositorioService, RepositorioService>();
builder.Services.AddCors();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapGet("/repos/me", async (IRepositorioService service) =>
{
    var result = await service.ListarRepositoriosDoUsuario("octocat");
    return Results.Ok(result);
});

app.MapPost("/favoritos", async (Favorito favorito, IRepositorioService service) =>
{
    await service.AdicionarFavorito(favorito);
    return Results.Ok();
});

app.MapGet("/favoritos", async (IRepositorioService service) =>
{
    var result = await service.ListarFavoritos();
    return Results.Ok(result);
});

app.Run();

record Favorito(string Nome, string Url);

interface IRepositorioService
{
    Task<List<object>> ListarRepositoriosDoUsuario(string usuario);
    Task AdicionarFavorito(Favorito favorito);
    Task<List<Favorito>> ListarFavoritos();
}

class RepositorioService : IRepositorioService
{
    private readonly HttpClient _http;

    public RepositorioService()
    {
        _http = new HttpClient();
        _http.DefaultRequestHeaders.UserAgent.ParseAdd("request");
    }

    public async Task<List<object>> ListarRepositoriosDoUsuario(string usuario)
    {
        // TODO
        // Seu código aqui
        throw new NotImplementedException("Implementar lógica para listar repositórios.");
    }

    public async Task AdicionarFavorito(Favorito favorito)
    {
        // TODO
        // Seu código aqui
    }

    public async Task<List<Favorito>> ListarFavoritos()
    {
        // TODO
        // Seu código aqui
        throw new NotImplementedException("Implementar lógica para listar favoritos.");
    }
}