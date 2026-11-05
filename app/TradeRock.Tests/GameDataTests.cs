using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging.Abstractions;
using TradeRock.App;
using TradeRock.Core;
using TradeRock.Core.Dex;
using TradeRock.Core.GameData;
using TradeRock.Core.Games;

namespace TradeRock.Tests;

public sealed class GameDataTests(DexFixture fx) : IClassFixture<DexFixture>
{
    private const string Source = "\"source\": [\"https://example.org\"]";

    private static GameDataStore FromFiles(IReadOnlyDictionary<string, string> files, Func<string, bool>? moveExists = null) =>
        new(files, moveExists, NullLogger.Instance);

    [Fact]
    public void Dados_embutidos_carregam_e_passam_no_validador()
    {
        var store = new GameDataStore(fx.Service, NullLogger<GameDataStore>.Instance);
        Assert.NotEmpty(store.ShinyMethods);
        Assert.DoesNotContain(store.Bosses("frlg"), b => b.Party.Count == 0);
    }

    [Fact]
    public void Masuda_vale_no_SV_e_tem_amuleto()
    {
        var store = FromFiles(GameDataStore.EmbeddedFiles());
        var masuda = Assert.Single(store.ShinyMethods, m => m.Id == "masuda");
        Assert.Contains("sv", masuda.Games);
        Assert.Contains(masuda.Modifiers, m => m.Id == "shiny_charm" && m.Kind == "toggle" && m.Odds is > 0);
        Assert.All(store.ShinyMethods, m => Assert.All(m.Games, g => GameVersions.ByKey(g)));
    }

    [Fact]
    public void Cada_arquivo_de_dados_desserializa()
    {
        var files = GameDataStore.EmbeddedFiles();
        Assert.Contains("shiny-methods.json", files.Keys);
        foreach (var (path, json) in files)
            GameDataStore.Parse(path, json); // lança se não desserializar
    }

    [Fact]
    public void Validador_rejeita_especie_inexistente()
    {
        var files = new Dictionary<string, string>
        {
            ["areas/frlg.json"] = $$"""
                { {{Source}}, "items": [ { "id": "route-1", "name": "Route 1", "kind": "route", "order": 1,
                  "map_node": null, "poke_api_location_id": null, "bosses_before": [] } ] }
                """,
            ["encounters/frlg.json"] = $$"""
                { {{Source}}, "items": [ { "area": "route-1", "species": 2000, "form": 0, "method": "walk",
                  "min_level": 2, "max_level": 4, "chance": "common", "conditions": null, "versions": ["firered"],
                  "alpha": false, "verified": true } ] }
                """,
        };
        var ex = Assert.Throws<InvalidDataException>(() => FromFiles(files));
        Assert.Contains("2000", ex.Message);
    }

    [Fact]
    public void Validador_confere_referencias_metodos_e_golpes()
    {
        var files = new Dictionary<string, string>
        {
            ["bosses/frlg.json"] = $$"""
                { {{Source}}, "items": [ { "id": "brock", "kind": "gym", "name": "Brock", "title": null, "location": null,
                  "order": 1, "badge": "boulder", "party": [ { "species": 74, "form": 0, "level": 12,
                  "moves": ["tackle", "not-a-move"], "ability": null, "item": null, "tera_type": null } ],
                  "level_cap": 14, "verified": true, "notes": null } ] }
                """,
            ["areas/frlg.json"] = $$"""
                { {{Source}}, "items": [ { "id": "pewter", "name": "Pewter", "kind": "city", "order": 1,
                  "map_node": "pewter", "poke_api_location_id": null, "bosses_before": ["misty"] } ] }
                """,
            ["encounters/frlg.json"] = $$"""
                { {{Source}}, "items": [ { "area": "nowhere", "species": 16, "form": 0, "method": "teleport",
                  "min_level": 2, "max_level": 4, "chance": "common", "conditions": null, "versions": ["firered"],
                  "alpha": false, "verified": true } ] }
                """,
            ["trainers/frlg.json"] = $$"""
                { {{Source}}, "items": [ { "id": "t1", "area": "pewter", "class": "Bug Catcher", "name": "Rick",
                  "party": [ { "species": 10, "form": 0, "level": 6, "moves": [], "ability": null, "item": null,
                  "tera_type": null } ], "verified": true } ] }
                """,
        };
        var ex = Assert.Throws<InvalidDataException>(() => FromFiles(files, m => m == "tackle"));
        Assert.Contains("not-a-move", ex.Message);
        Assert.Contains("level_cap", ex.Message);  // 14 em vez de 12
        Assert.Contains("misty", ex.Message);      // chefe que não existe
        Assert.Contains("pewter", ex.Message);     // nó que não existe no mapa de Kanto
        Assert.Contains("nowhere", ex.Message);    // área que não existe
        Assert.Contains("teleport", ex.Message);   // método fora da lista
        Assert.DoesNotContain("t1", ex.Message);   // o treinador está certo

        // Sem o dex (moveExists nulo), os golpes não são conferidos.
        var noDex = Assert.Throws<InvalidDataException>(() => FromFiles(files));
        Assert.DoesNotContain("not-a-move", noDex.Message);
    }

    [Fact]
    public void Validador_rejeita_jogo_desconhecido_nos_metodos()
    {
        var files = new Dictionary<string, string>
        {
            ["shiny-methods.json"] = $$"""
                { {{Source}}, "items": [ { "id": "x", "name": "X", "description": "d", "base_odds": 4096,
                  "modifiers": [], "games": ["zz"], "notes": null, "verified": false } ] }
                """,
        };
        var ex = Assert.Throws<InvalidDataException>(() => FromFiles(files));
        Assert.Contains("zz", ex.Message);
    }

    [Theory]
    [InlineData("""{ "source": [], "items": [ { "id": "x", "name": "X", "description": "d", "base_odds": 4096, "modifiers": [], "games": ["sv"], "notes": null } ] }""")]
    [InlineData("""{ "source": [], "items": [ { "id": "x", "name": "X", "description": "d", "base_odds": 4096, "modifiers": [], "games": ["sv"], "notes": null, "verified": true, "extra": 1 } ] }""")]
    [InlineData("""{ "source": [], "items": [ { "id": "x", "name": null, "description": "d", "base_odds": 4096, "modifiers": [], "games": ["sv"], "notes": null, "verified": true } ] }""")]
    [InlineData("""{ "items": [] }""")]
    public void Arquivo_fora_do_esquema_falha(string json)
    {
        Assert.ThrowsAny<Exception>(() => GameDataStore.Parse("shiny-methods.json", json));
        Assert.Throws<InvalidDataException>(() => FromFiles(new Dictionary<string, string> { ["shiny-methods.json"] = json }));
    }

    [Fact]
    public void Jogo_sem_arquivo_da_lista_vazia_e_mapa_valido_carrega()
    {
        var files = new Dictionary<string, string>
        {
            ["maps/kanto.json"] = $$"""
                { {{Source}}, "region": "kanto", "nodes": [ { "id": "pallet", "name": "Pallet Town", "kind": "city",
                  "x": 0.1, "y": 0.9, "games": null }, { "id": "route-1", "name": "Route 1", "kind": "route",
                  "x": 0.1, "y": 0.8, "games": ["frlg"] } ], "edges": [ { "from": "pallet", "to": "route-1" } ] }
                """,
            ["trainers/frlg.json"] = $$"""{ {{Source}}, "items": [] }""",
        };
        var store = FromFiles(files);
        Assert.Empty(store.Bosses("sv"));
        Assert.Empty(store.Areas("sv"));
        Assert.Empty(store.Encounters("sv"));
        Assert.Empty(store.GamesWithTrainers);
        Assert.Null(store.Map(Region.Johto));
        var kanto = store.Map(Region.Kanto);
        Assert.NotNull(kanto);
        Assert.Equal(2, kanto.Nodes.Count);
    }

    [Fact]
    public void Mapa_com_aresta_para_no_inexistente_falha()
    {
        var files = new Dictionary<string, string>
        {
            ["maps/kanto.json"] = $$"""
                { {{Source}}, "region": "kanto", "nodes": [ { "id": "pallet", "name": "Pallet Town", "kind": "city",
                  "x": 0, "y": 0, "games": null } ], "edges": [ { "from": "pallet", "to": "viridian" } ] }
                """,
        };
        var ex = Assert.Throws<InvalidDataException>(() => FromFiles(files));
        Assert.Contains("viridian", ex.Message);
    }
}

public sealed class GameDataApiTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private readonly string _dir = Directory.CreateTempSubdirectory("traderock-dataapi-").FullName;
    private WebApplication? _app;
    private HttpClient _client = null!;

    public async ValueTask InitializeAsync()
    {
        _app = TradeRockServer.Create(["--data-dir", _dir], _token, b => b.WebHost.UseTestServer());
        await _app.StartAsync();
        _client = _app.GetTestClient();
        _client.BaseAddress = new Uri("http://127.0.0.1/");
        _client.DefaultRequestHeaders.Add(SessionAuthMiddleware.HeaderName, _token.Value);
    }

    public async ValueTask DisposeAsync()
    {
        if (_app is not null)
            await _app.DisposeAsync();
        SqliteConnection.ClearAllPools();
        try { Directory.Delete(_dir, true); } catch (IOException) { }
    }

    [Fact]
    public async Task Rotas_de_dados()
    {
        var all = await _client.GetFromJsonAsync<JsonElement>("/api/data/shiny-methods");
        var sv = await _client.GetFromJsonAsync<JsonElement>("/api/data/shiny-methods?game=sv");
        Assert.True(all.GetArrayLength() > sv.GetArrayLength());
        var masuda = sv.EnumerateArray().Single(m => m.GetProperty("id").GetString() == "masuda");
        Assert.Equal(683.08, masuda.GetProperty("baseOdds").GetDouble(), 2);
        Assert.All(sv.EnumerateArray(), m =>
            Assert.Contains("sv", m.GetProperty("games").EnumerateArray().Select(g => g.GetString())));

        Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync("/api/data/shiny-methods?game=zz")).StatusCode);
        foreach (var kind in new[] { "bosses", "trainers", "areas", "encounters" })
        {
            var r = await _client.GetAsync($"/api/data/frlg/{kind}");
            Assert.Equal(HttpStatusCode.OK, r.StatusCode);
            Assert.Equal(JsonValueKind.Array, (await r.Content.ReadFromJsonAsync<JsonElement>()).ValueKind);
            Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync($"/api/data/zz/{kind}")).StatusCode);
        }
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync("/api/data/maps/atlantis")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync("/api/data/maps/johto")).StatusCode);
    }
}
