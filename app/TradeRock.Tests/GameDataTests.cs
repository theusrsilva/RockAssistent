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

    // Gen 1–3: os chefes da história (até o campeão) têm 8 líderes com insígnia, 4 da Elite 4 e 1 campeão; o resto
    // (Kanto no GSC, revanches, Frontier Brains) vem depois, com Order maior. "drops" lista as quedas de nível
    // entre ginásios seguidos que o jogo tem de verdade (no GSC, a Jasmine vem antes do Pryce e tem time mais forte).
    [Theory]
    [InlineData("rb", "")]
    [InlineData("y", "")]
    [InlineData("gs", "jasmine>pryce")]
    [InlineData("c", "jasmine>pryce")]
    [InlineData("rs", "")]
    [InlineData("e", "")]
    [InlineData("frlg", "")]
    public void Chefes_da_historia_das_geracoes_1_a_3(string game, string drops)
    {
        var bosses = FromFiles(GameDataStore.EmbeddedFiles()).Bosses(game).OrderBy(b => b.Order).ToList();
        Assert.Equal(bosses.Count, bosses.Select(b => b.Order).Distinct().Count());
        var champion = Assert.Single(bosses, b => b.Kind == "champion" && b.Order == bosses.Where(x => x.Kind == "champion").Min(x => x.Order));
        var story = bosses.Where(b => b.Order <= champion.Order).ToList();

        var gyms = story.Where(b => b.Kind == "gym").ToList();
        Assert.Equal(8, gyms.Count);
        Assert.Equal(8, gyms.Select(g => g.Badge).Distinct().Count());
        Assert.All(bosses.Where(b => b.Kind == "gym"), g => Assert.False(string.IsNullOrWhiteSpace(g.Badge)));
        Assert.Equal(4, story.Count(b => b.Kind == "elite"));
        Assert.Single(story, b => b.Kind == "champion");
        Assert.True(story.Where(b => b.Kind == "elite").All(e => e.Order < champion.Order));

        var found = gyms.Zip(gyms.Skip(1)).Where(p => p.Second.LevelCap < p.First.LevelCap)
            .Select(p => $"{p.First.Id}>{p.Second.Id}");
        Assert.Equal(drops, string.Join(",", found));

        var rivals = bosses.Where(b => b.Kind == "rival").ToList();
        Assert.True(rivals.Count >= 3);
        Assert.All(rivals.Zip(rivals.Skip(1)), p => Assert.True(p.Second.LevelCap >= p.First.LevelCap, $"{p.First.Id} -> {p.Second.Id}"));
        Assert.True(rivals.Max(r => r.Order) < champion.Order || game is "gs" or "c");
    }

    [Fact]
    public void Validador_rejeita_jogo_desconhecido_nos_metodos()
    {
        var files = new Dictionary<string, string>
        {
            ["shiny-methods.json"] = $$"""
                { {{Source}}, "items": [ { "id": "x", "name": "X", "description": "d", "base_odds": 4096,
                  "base_rolls": null, "roll_odds": null, "modifiers": [], "games": ["zz"], "notes": null, "verified": false } ] }
                """,
        };
        var ex = Assert.Throws<InvalidDataException>(() => FromFiles(files));
        Assert.Contains("zz", ex.Message);
    }

    private static double Combine(ShinyMethod m, params int[] extra) =>
        ShinyOdds.FromRolls(m.RollOdds!.Value, m.BaseRolls!.Value + extra.Sum());

    private static int Rolls(ShinyMethod m, string modifier, string? step = null)
    {
        var mod = m.Modifiers.Single(x => x.Id == modifier);
        return step is null ? mod.Rolls!.Value : mod.Steps!.Single(s => s.Label == step).Rolls!.Value;
    }

    [Fact]
    public void Regra_das_rolagens_reproduz_as_odds_das_fontes()
    {
        var store = FromFiles(GameDataStore.EmbeddedFiles());
        ShinyMethod M(string id) => store.ShinyMethods.Single(m => m.Id == id);

        var sos = M("sos_chain");
        Assert.Equal(273.53, Combine(sos, Rolls(sos, "chain", "31+"), Rolls(sos, "shiny_charm")), 0.01);
        var brilliant = M("brilliant");
        Assert.Equal(455.56, Combine(brilliant, Rolls(brilliant, "battled", "500+"), Rolls(brilliant, "shiny_charm")), 0.01);
        var outbreak = M("pla_outbreak");
        Assert.Equal(137.02, Combine(outbreak, Rolls(outbreak, "shiny_charm"), Rolls(outbreak, "research_10")), 0.01);
        var sv = M("sv_wild");
        Assert.Equal(512.44, Combine(sv, Rolls(sv, "sparkling_power", "nível 3"), Rolls(sv, "outbreak", "60+"), Rolls(sv, "shiny_charm")), 0.01);

        // Sem rolagens: não combina.
        Assert.Null(M("dynamax_adventure").BaseRolls);
        Assert.All(M("dynamax_adventure").Modifiers, x => Assert.Null(x.Rolls));
        // Amuleto só no B2W2 nos métodos da Gen 5.
        Assert.Equal(["b2w2"], M("masuda_gen5").Modifiers.Single(x => x.Id == "shiny_charm").Games);
        Assert.All(store.ShinyMethods, m => Assert.True(m.Modifiers.Count(x => x.Counter) <= 1));
        Assert.True(M("sos_chain").Modifiers.Single(x => x.Id == "chain").Counter);
    }

    private static Dictionary<string, string> Shiny(string method) => new()
    {
        ["shiny-methods.json"] = $$"""{ "source": [], "items": [ {{method}} ] }""",
    };

    public static TheoryData<string, string> MetodosInvalidos { get; } = new()
    {
        {
            """
            { "id": "a", "name": "A", "description": "d", "base_odds": 4096, "base_rolls": 1, "roll_odds": 4096, "games": ["sv"], "notes": null, "verified": true, "modifiers": [ { "id": "c", "name": "C", "kind": "toggle", "steps": null, "odds": 1000, "rolls": 2, "counter": false, "games": null } ] }
            """,
            "não batem"
        },
        {
            """
            { "id": "a", "name": "A", "description": "d", "base_odds": 4096, "base_rolls": null, "roll_odds": null, "games": ["sv"], "notes": null, "verified": true, "modifiers": [ { "id": "x", "name": "X", "kind": "steps", "steps": [ { "label": "1", "min": 0, "max": null, "odds": 10, "rolls": null } ], "odds": null, "rolls": null, "counter": true, "games": null }, { "id": "y", "name": "Y", "kind": "steps", "steps": [ { "label": "1", "min": 0, "max": null, "odds": 10, "rolls": null } ], "odds": null, "rolls": null, "counter": true, "games": null } ] }
            """,
            "counter"
        },
        {
            """
            { "id": "a", "name": "A", "description": "d", "base_odds": 4096, "base_rolls": null, "roll_odds": null, "games": ["sv"], "notes": null, "verified": true, "modifiers": [ { "id": "c", "name": "C", "kind": "toggle", "steps": null, "odds": 100, "rolls": null, "counter": false, "games": ["swsh"] } ] }
            """,
            "swsh"
        },
        {
            """
            { "id": "a", "name": "A", "description": "d", "base_odds": 4096, "base_rolls": 1, "roll_odds": 4096, "games": ["sv"], "notes": null, "verified": true, "modifiers": [ { "id": "c", "name": "C", "kind": "toggle", "steps": null, "odds": 100, "rolls": null, "counter": false, "games": null } ] }
            """,
            "rolls"
        },
    };

    [Theory]
    [MemberData(nameof(MetodosInvalidos))]
    public void Validador_confere_rolagens_contador_e_jogos_dos_modificadores(string method, string expected)
    {
        var ex = Assert.Throws<InvalidDataException>(() => FromFiles(Shiny(method)));
        Assert.Contains(expected, ex.Message);
    }

    [Theory]
    [InlineData("""{ "source": [], "items": [ { "id": "x", "name": "X", "description": "d", "base_odds": 4096, "base_rolls": null, "roll_odds": null, "modifiers": [], "games": ["sv"], "notes": null } ] }""")]
    [InlineData("""{ "source": [], "items": [ { "id": "x", "name": "X", "description": "d", "base_odds": 4096, "base_rolls": null, "roll_odds": null, "modifiers": [], "games": ["sv"], "notes": null, "verified": true, "extra": 1 } ] }""")]
    [InlineData("""{ "source": [], "items": [ { "id": "x", "name": null, "description": "d", "base_odds": 4096, "base_rolls": null, "roll_odds": null, "modifiers": [], "games": ["sv"], "notes": null, "verified": true } ] }""")]
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
