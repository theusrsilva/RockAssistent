using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using TradeRock.App;
using TradeRock.Core;

namespace TradeRock.Tests;

/// <summary>GET /api/sprites/{species}?shiny=1&amp;form=&lt;pokemonId&gt;: sprites embutidos e a ordem das alternativas.</summary>
public sealed class SpritesTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private WebApplication _app = null!;
    private string _dataDir = null!;
    private HttpClient _client = null!;

    public async ValueTask InitializeAsync()
    {
        _dataDir = Directory.CreateTempSubdirectory("traderock-sprites-").FullName;
        _app = TradeRockServer.Create(["--data-dir", _dataDir], _token, b => b.WebHost.UseTestServer());
        await _app.StartAsync();
        _client = _app.GetTestClient();
        _client.BaseAddress = new Uri("http://127.0.0.1/");
        _client.DefaultRequestHeaders.Add(SessionAuthMiddleware.HeaderName, _token.Value);
    }

    public async ValueTask DisposeAsync()
    {
        _client.Dispose();
        await _app.DisposeAsync();
        try
        {
            Directory.Delete(_dataDir, recursive: true);
        }
        catch (IOException)
        {
        }
    }

    private static byte[] Resource(string name)
    {
        using var stream = typeof(TradeRockServer).Assembly.GetManifestResourceStream(name);
        Assert.NotNull(stream);
        using var copy = new MemoryStream();
        stream.CopyTo(copy);
        return copy.ToArray();
    }

    private async Task<byte[]> GetAsync(string url)
    {
        var response = await _client.GetAsync(url);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("image/png", response.Content.Headers.ContentType?.MediaType);
        return await response.Content.ReadAsByteArrayAsync();
    }

    [Fact]
    public async Task Shiny_existe_para_o_Gengar()
    {
        var shiny = await GetAsync("/api/sprites/94?shiny=1");
        Assert.Equal(Resource("sprites/shiny/94.png"), shiny);
        Assert.NotEqual(Resource("sprites/94.png"), shiny);
        Assert.Equal(Resource("sprites/94.png"), await GetAsync("/api/sprites/94"));
        Assert.Equal(Resource("sprites/94.png"), await GetAsync("/api/sprites/94?shiny=0"));
    }

    [Fact]
    public void Todas_as_especies_tem_sprite_e_shiny()
    {
        var assembly = typeof(TradeRockServer).Assembly;
        for (var n = 1; n <= TradeRock.App.Api.AppEndpoints.MaxSpriteSpecies; n++)
        {
            Assert.NotNull(assembly.GetManifestResourceInfo($"sprites/{n}.png"));
            Assert.NotNull(assembly.GetManifestResourceInfo($"sprites/shiny/{n}.png"));
        }
    }

    [Fact]
    public async Task Forma_escolhe_o_sprite_da_forma()
    {
        // 10034 = Mega Charizard X (espécie 6).
        Assert.Equal(Resource("sprites/forms/10034.png"), await GetAsync("/api/sprites/6?form=10034"));
        Assert.Equal(Resource("sprites/forms/shiny/10034.png"), await GetAsync("/api/sprites/6?form=10034&shiny=1"));
    }

    [Fact]
    public async Task Sem_arquivo_cai_no_proximo_da_fila()
    {
        // 10264 = Koraidon Limited Build (espécie 1007): o PokeAPI/sprites não tem o arquivo. 99999 não existe.
        Assert.Null(typeof(TradeRockServer).Assembly.GetManifestResourceInfo("sprites/forms/10264.png"));
        Assert.Equal(Resource("sprites/1007.png"), await GetAsync("/api/sprites/1007?form=10264"));
        Assert.Equal(Resource("sprites/shiny/1007.png"), await GetAsync("/api/sprites/1007?form=10264&shiny=1"));
        Assert.Equal(Resource("sprites/25.png"), await GetAsync("/api/sprites/25?form=99999"));
    }

    [Fact]
    public async Task Fora_da_faixa_continua_404()
    {
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync("/api/sprites/0?shiny=1")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync("/api/sprites/1026?form=10034")).StatusCode);
    }
}
