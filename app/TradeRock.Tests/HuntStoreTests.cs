using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Logging.Abstractions;
using TradeRock.App;
using TradeRock.Core;
using TradeRock.Core.GameData;
using TradeRock.Core.Games;
using TradeRock.Core.Hunts;
using TradeRock.Core.Storage;

namespace TradeRock.Tests;

public sealed class HuntStoreTests(DexFixture fx) : IClassFixture<DexFixture>, IAsyncLifetime
{
    private readonly TestDataDir _dir = new();
    private IDbContextFactory<TradeRockDb> _db = null!;
    private HuntStore _store = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        _db = await _dir.CreateDbFactoryAsync();
        _store = new HuntStore(_db, new GameDataStore(fx.Service, NullLogger<GameDataStore>.Instance), fx.Service, TimeProvider.System);
    }

    public ValueTask DisposeAsync()
    {
        SqliteConnection.ClearAllPools();
        _dir.Dispose();
        return ValueTask.CompletedTask;
    }

    private Task<HuntView> NewMasuda() =>
        _store.CreateAsync(new NewHunt(906, 0, "sv", "masuda", new Dictionary<string, string> { ["shiny_charm"] = "true" }), Ct);

    [Fact]
    public async Task Cria_e_calcula_as_odds()
    {
        var hunt = await NewMasuda();
        Assert.Equal(512.44, hunt.Odds, 0.01);
        Assert.Equal(0, hunt.Cumulative);
        Assert.Equal("true", hunt.Modifiers["shiny_charm"]);

        var updated = await _store.UpdateAsync(hunt.Id, new HuntChanges(355, null, "  ovos do Sprigatito ", 60), Ct);
        Assert.Equal(0.50, updated.Cumulative, 0.01);
        Assert.Equal("ovos do Sprigatito", updated.Notes);
        Assert.Equal(60, updated.ElapsedSeconds);
    }

    [Fact]
    public async Task Incremento_negativo_nao_passa_de_zero()
    {
        var hunt = await NewMasuda();
        Assert.Equal(3, (await _store.IncrementAsync(hunt.Id, 3, Ct)).Count);
        Assert.Equal(0, (await _store.IncrementAsync(hunt.Id, -100, Ct)).Count);
        foreach (var delta in new[] { 0, 101, -101 })
            Assert.Equal("invalid_delta", (await Assert.ThrowsAsync<HuntException>(() => _store.IncrementAsync(hunt.Id, delta, Ct))).Code);
    }

    [Fact]
    public async Task Incrementos_em_paralelo_nao_se_perdem()
    {
        var hunt = await NewMasuda();
        await Task.WhenAll(Enumerable.Range(0, 20).Select(_ => Task.Run(() => _store.IncrementAsync(hunt.Id, 1, Ct), Ct)));
        Assert.Equal(20, (await _store.GetAsync(hunt.Id, Ct)).Count);
    }

    [Fact]
    public async Task Amuleto_no_Arceus_implica_pesquisa_nivel_10()
    {
        var charm = new Dictionary<string, string> { ["shiny_charm"] = "true" };
        var hunt = await _store.CreateAsync(new NewHunt(399, 0, "pla", "pla_wild", charm), Ct);
        Assert.Equal("true", hunt.Modifiers["research_10"]);
        Assert.Equal(819.60, hunt.Odds, 0.01);

        // Com a pesquisa perfeita ligada, o nível 10 não é forçado (ela já o inclui).
        var perfect = await _store.UpdateAsync(hunt.Id, new HuntChanges(null,
            new Dictionary<string, string> { ["shiny_charm"] = "true", ["research_perfect"] = "true" }, null, null), Ct);
        Assert.False(perfect.Modifiers.ContainsKey("research_10"));
    }

    [Fact]
    public async Task Especie_invalida_na_lista_de_jogos()
    {
        foreach (var species in new[] { 0, 1026 })
            Assert.Equal("invalid_species", (await Assert.ThrowsAsync<HuntException>(() => _store.GamesForSpeciesAsync(species, Ct))).Code);
    }

    [Fact]
    public async Task Finish_grava_EndedAt_e_trava_o_contador()
    {
        var hunt = await NewMasuda();
        var done = await _store.FinishAsync(hunt.Id, found: true, 3600, Ct);
        Assert.NotNull(done.EndedAt);
        Assert.Equal(3600, done.ElapsedSeconds);
        Assert.True(done.Found);

        var ex = await Assert.ThrowsAsync<HuntException>(() => _store.IncrementAsync(hunt.Id, 1, Ct));
        Assert.Equal(("hunt_finished", 409), (ex.Code, ex.Status));
        Assert.Empty(await _store.ListAsync(all: false, Ct));
        Assert.Single(await _store.ListAsync(all: true, Ct));
    }

    [Fact]
    public async Task Regras_de_criacao()
    {
        async Task<string> Fails(NewHunt h) => (await Assert.ThrowsAsync<HuntException>(() => _store.CreateAsync(h, Ct))).Code;

        Assert.Equal("unknown_game", await Fails(new NewHunt(25, 0, "zz", "masuda", null)));
        Assert.Equal("method_not_in_game", await Fails(new NewHunt(25, 0, "frlg", "masuda", null)));
        Assert.Equal("unknown_method", await Fails(new NewHunt(25, 0, "sv", "nada", null)));
        Assert.Equal("invalid_species", await Fails(new NewHunt(0, 0, "sv", "masuda", null)));
        Assert.Equal("unknown_modifier", await Fails(new NewHunt(25, 0, "sv", "masuda", new Dictionary<string, string> { ["x"] = "true" })));
        Assert.Equal("invalid_modifier", await Fails(new NewHunt(25, 0, "sv", "masuda", new Dictionary<string, string> { ["shiny_charm"] = "sim" })));
        Assert.Equal("modifier_not_in_game", await Fails(new NewHunt(25, 0, "bw", "masuda_gen5",
            new Dictionary<string, string> { ["shiny_charm"] = "true" })));
    }

    [Fact]
    public async Task Hunt_de_outro_dono_nao_aparece()
    {
        var other = Guid.NewGuid();
        HuntView theirs;
        using (OwnerScope.Use(other))
        {
            theirs = await NewMasuda();
            Assert.Single(await _store.ListAsync(true, Ct));
        }

        Assert.Empty(await _store.ListAsync(true, Ct));
        Assert.Equal(404, (await Assert.ThrowsAsync<HuntException>(() => _store.IncrementAsync(theirs.Id, 1, Ct))).Status);
        Assert.Equal(404, (await Assert.ThrowsAsync<HuntException>(() => _store.DeleteAsync(theirs.Id, Ct))).Status);

        await using var db = await _db.CreateDbContextAsync(Ct);
        Assert.Equal(other, (await db.Hunts.IgnoreQueryFilters().SingleAsync(Ct)).OwnerId);
    }

    [Fact]
    public async Task Jogos_da_especie_filtram_os_metodos()
    {
        var games = await _store.GamesForSpeciesAsync(906, Ct); // Sprigatito: só no SV
        var sv = Assert.Single(games);
        Assert.Equal("sv", sv.GameKey);
        Assert.Contains("egg", sv.Kinds);
        Assert.Contains(sv.Methods, m => m.Id == "masuda");
        Assert.Contains(sv.Methods, m => m.Id == "full_odds");

        var mewtwo = await _store.GamesForSpeciesAsync(150, Ct); // não bota ovo
        Assert.All(mewtwo, g => Assert.DoesNotContain(g.Methods, m => m.Id.StartsWith("masuda", StringComparison.Ordinal)));
        Assert.DoesNotContain(mewtwo, g => g.Kinds.Contains("egg"));

        // Amuleto do Masuda da Gen 5 só no B2W2.
        var pikachu = await _store.GamesForSpeciesAsync(25, Ct);
        Assert.DoesNotContain(pikachu.Single(g => g.GameKey == "bw").Methods.Single(m => m.Id == "masuda_gen5").Modifiers,
            x => x.Id == "shiny_charm");
        Assert.Contains(pikachu.Single(g => g.GameKey == "b2w2").Methods.Single(m => m.Id == "masuda_gen5").Modifiers,
            x => x.Id == "shiny_charm");
        // Ovo Estranho só para os bebês do Crystal.
        Assert.DoesNotContain(pikachu.Single(g => g.GameKey == "c").Methods, m => m.Id == "odd_egg");
        Assert.Contains((await _store.GamesForSpeciesAsync(172, Ct)).Single(g => g.GameKey == "c").Methods, m => m.Id == "odd_egg");
    }

    [Fact]
    public async Task Encontro_dos_nossos_dados_entra()
    {
        // Os encontros do BDSP, do Arceus, do SV e do Z-A vêm de assets/data/encounters (o PokeAPI não tem).
        var store = new GameDataStore(fx.Service, NullLogger<GameDataStore>.Instance);
        var wild = store.Encounters("pla").First(e => e.Method == "walk");
        var pla = (await _store.GamesForSpeciesAsync(wild.Species, Ct)).Single(g => g.GameKey == "pla");
        Assert.Contains("encounter", pla.Kinds);
        Assert.Contains(pla.Methods, m => m.Id == "pla_outbreak");
    }
}

public sealed class HuntMigrationTests
{
    [Fact]
    public async Task Migracao_sobre_banco_antigo_mantem_o_cofre()
    {
        using var dir = new TestDataDir();
        var options = new DbContextOptionsBuilder<TradeRockDb>()
            .UseSqlite($"Data Source={dir.Paths.DatabasePath};Pooling=False").Options;
        var ct = TestContext.Current.CancellationToken;

        await using (var db = new TradeRockDb(options))
        {
            await db.GetService<IMigrator>().MigrateAsync("SaveAttachments", ct);
            db.Pokemon.Add(new StoredPokemon
            {
                Id = Guid.NewGuid(), Sha256 = "ab", Size = 100, Species = 25, SpeciesName = "Pikachu", Nickname = "PIKA",
                OtName = "RED", OriginGame = "FireRed", AddedAt = DateTimeOffset.UtcNow, Family = GameFamily.Frlg,
            });
            await db.SaveChangesAsync(ct);
        }

        await using (var db = new TradeRockDb(options))
        {
            await db.Database.MigrateAsync(ct);
            Assert.Equal(1, await db.Pokemon.CountAsync(ct));
            Assert.Equal(OwnerScope.LocalOwner, (await db.Pokemon.SingleAsync(ct)).OwnerId);
            db.Hunts.Add(new ShinyHunt { GameKey = "sv", MethodId = "masuda", ModifiersJson = "{}", Species = 906 });
            await db.SaveChangesAsync(ct);
            Assert.Equal(OwnerScope.LocalOwner, (await db.Hunts.SingleAsync(ct)).OwnerId);
        }
    }
}

public sealed class HuntApiTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private readonly string _dir = Directory.CreateTempSubdirectory("traderock-hunt-").FullName;
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

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
        _client.Dispose();
        await _app.DisposeAsync();
        SqliteConnection.ClearAllPools();
        try { Directory.Delete(_dir, true); } catch (IOException) { }
    }

    private async Task<JsonElement> Ok(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync(Ct);
        Assert.True(response.IsSuccessStatusCode, $"{(int)response.StatusCode}: {text}");
        return text.Length == 0 ? default : JsonDocument.Parse(text).RootElement.Clone();
    }

    [Fact]
    public async Task Fluxo_da_hunt_pela_api()
    {
        var hunt = await Ok(await _client.PostAsJsonAsync("/api/hunts",
            new { species = 906, form = 0, gameKey = "sv", methodId = "masuda", modifiers = new { shiny_charm = "true" } }, Ct));
        var id = hunt.GetProperty("id").GetGuid();
        Assert.Equal(512.44, hunt.GetProperty("odds").GetDouble(), 0.01);
        Assert.Equal("true", hunt.GetProperty("modifiers").GetProperty("shiny_charm").GetString());

        var inc = await Ok(await _client.PostAsJsonAsync($"/api/hunts/{id}/increment", new { delta = 5 }, Ct));
        Assert.Equal(5, inc.GetProperty("count").GetInt32());
        var bad = await _client.PostAsJsonAsync($"/api/hunts/{id}/increment", new { delta = 500 }, Ct);
        Assert.Equal(HttpStatusCode.BadRequest, bad.StatusCode);
        Assert.Contains("invalid_delta", await bad.Content.ReadAsStringAsync(Ct));

        var put = await Ok(await _client.PutAsJsonAsync($"/api/hunts/{id}", new { notes = "noite", elapsedSeconds = 90 }, Ct));
        Assert.Equal("noite", put.GetProperty("notes").GetString());
        Assert.Equal(5, put.GetProperty("count").GetInt32());

        Assert.Equal(1, (await Ok(await _client.GetAsync("/api/hunts", Ct))).GetArrayLength());
        var done = await Ok(await _client.PostAsJsonAsync($"/api/hunts/{id}/finish", new { found = true, elapsedSeconds = 120 }, Ct));
        Assert.Equal(JsonValueKind.String, done.GetProperty("endedAt").ValueKind);
        Assert.Equal(120, done.GetProperty("elapsedSeconds").GetInt64());
        Assert.Equal(HttpStatusCode.Conflict, (await _client.PostAsJsonAsync($"/api/hunts/{id}/increment", new { delta = 1 }, Ct)).StatusCode);
        Assert.Equal(0, (await Ok(await _client.GetAsync("/api/hunts", Ct))).GetArrayLength());
        Assert.Equal(1, (await Ok(await _client.GetAsync("/api/hunts?all=1", Ct))).GetArrayLength());

        Assert.Equal(HttpStatusCode.NoContent, (await _client.DeleteAsync($"/api/hunts/{id}", Ct)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.DeleteAsync($"/api/hunts/{id}", Ct)).StatusCode);

        var wrong = await _client.PostAsJsonAsync("/api/hunts", new { species = 25, form = 0, gameKey = "frlg", methodId = "masuda" }, Ct);
        Assert.Equal(HttpStatusCode.BadRequest, wrong.StatusCode);
        Assert.Contains("method_not_in_game", await wrong.Content.ReadAsStringAsync(Ct));
        var unknown = await _client.PostAsJsonAsync("/api/hunts", new { species = 25, form = 0, gameKey = "zz", methodId = "masuda" }, Ct);
        Assert.Contains("unknown_game", await unknown.Content.ReadAsStringAsync(Ct));
    }

    [Fact]
    public async Task Jogos_da_especie_pela_api()
    {
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync("/api/hunts/species/0/games", Ct)).StatusCode);
        var games = await Ok(await _client.GetAsync("/api/hunts/species/906/games", Ct));
        var sv = Assert.Single(games.EnumerateArray());
        Assert.Equal("sv", sv.GetProperty("gameKey").GetString());
        Assert.Contains("egg", sv.GetProperty("kinds").EnumerateArray().Select(k => k.GetString()));
        var masuda = sv.GetProperty("methods").EnumerateArray().Single(m => m.GetProperty("id").GetString() == "masuda");
        Assert.Equal(683.08, masuda.GetProperty("baseOdds").GetDouble(), 0.01);
        Assert.Equal("shiny_charm", masuda.GetProperty("modifiers")[0].GetProperty("id").GetString());
        Assert.Equal("Masuda Method", masuda.GetProperty("nameEn").GetString());
        Assert.Equal("Shiny Charm", masuda.GetProperty("modifiers")[0].GetProperty("nameEn").GetString());
    }
}
