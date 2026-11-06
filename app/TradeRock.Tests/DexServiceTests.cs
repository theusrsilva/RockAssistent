using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using TradeRock.App;
using TradeRock.Core;
using TradeRock.Core.Dex;

namespace TradeRock.Tests;

/// <summary>Uma cópia do dex.sqlite embutido para a classe toda (são 25 MB).</summary>
public sealed class DexFixture : IDisposable
{
    public string Dir { get; } = Directory.CreateTempSubdirectory("traderock-dex-").FullName;
    public DexDatabase Db { get; }
    public DexService Service { get; }

    public DexFixture()
    {
        Db = new DexDatabase(new AppPaths(Dir));
        Service = new DexService(Db);
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();
        try { Directory.Delete(Dir, true); } catch (IOException) { }
    }
}

public sealed class DexServiceTests(DexFixture fx) : IClassFixture<DexFixture>
{
    private DexService Dex
    {
        get
        {
            Assert.SkipUnless(DexDatabase.ResourceExists, "Falta o dex.sqlite embutido (rode scripts/build-dex.py).");
            return fx.Service;
        }
    }

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    [Fact]
    public async Task Maquinas_do_FireRed_do_SwSh_e_do_Arceus()
    {
        var frlg = await Dex.MachinesAsync("frlg", Ct);
        Assert.Equal(58, frlg.Count);
        var tm30 = Assert.Single(frlg, m => m.Machine == "TM30");
        Assert.Equal("Shadow Ball", tm30.MoveName);
        Assert.Equal("ghost", tm30.Type);
        Assert.Contains(frlg, m => m.Machine == "HM01" && m.MoveIdentifier == "cut");
        Assert.Equal(frlg.Select(m => m.Number).Order(), frlg.Select(m => m.Number));

        var swsh = await Dex.MachinesAsync("swsh", Ct);
        Assert.Contains(swsh, m => m.Machine == "TR99" && m.MoveName == "Body Press");
        Assert.Empty(await Dex.MachinesAsync("pla", Ct));
    }

    [Fact]
    public async Task Gengar_no_FireRed()
    {
        var g = await Dex.GetAsync(94, "frlg", "pt-BR", Ct);
        Assert.NotNull(g);
        Assert.True(g.ExistsInGame);
        Assert.Equal(["ghost", "poison"], g.Types);
        Assert.Equal("Gengar", g.Name);
        Assert.Equal("Gengar", g.Names["en"]);
        var ability = Assert.Single(g.Abilities);
        Assert.Equal("Levitate", ability.Name);
        Assert.False(ability.IsHidden);

        Assert.Equal([92, 93, 94], g.EvolutionChain.Select(s => s.SpeciesId));
        var haunter = g.EvolutionChain.Single(s => s.SpeciesId == 93);
        Assert.Equal(92, haunter.FromSpeciesId);
        Assert.Equal("level 25", Assert.Single(haunter.Conditions));
        Assert.Equal("trade", Assert.Single(g.EvolutionChain.Single(s => s.SpeciesId == 94).Conditions));

        Assert.Equal(2, g.DamageTaken["ground"]);
        Assert.Equal(0, g.DamageTaken["normal"]);
        Assert.False(g.DamageTaken.ContainsKey("fairy")); // a Fada só existe a partir da Gen 6
        Assert.Equal(17, g.DamageTaken.Count);

        Assert.Contains(g.Moves.LevelUp, m => m.Name == "Shadow Punch" && m.Level == 25);
        Assert.Contains(g.Moves.Machine, m => m.Name == "Shadow Ball" && m.MachineNumber == 30);
        Assert.NotEmpty(g.Moves.Tutor);
        Assert.Equal(60, g.Stats.Hp);
        Assert.Equal(500, g.Stats.Total);
        Assert.Equal(3, g.Stats.Evs["special-attack"]);
        Assert.Equal(["firered", "leafgreen"], g.FlavorTexts.Select(f => f.Version));
        Assert.DoesNotContain(g.Forms, f => f.IsMega); // a Mega só chegou no X/Y
        Assert.Equal(94, g.DexNumbers["kanto"]);
        Assert.Empty(g.Encounters); // Gengar não aparece na natureza do FRLG
    }

    [Fact]
    public async Task Golpes_com_os_valores_da_geracao()
    {
        var tackle = await Dex.GetMoveAsync(33, "frlg", Ct);
        Assert.NotNull(tackle);
        Assert.Equal(35, tackle.Power);
        Assert.Equal(95, tackle.Accuracy);
        var now = await Dex.GetMoveAsync(33, "sv", Ct);
        Assert.Equal(40, now!.Power);
        var charm = await Dex.GetMoveAsync(204, "frlg", Ct);
        Assert.Equal("normal", charm!.Type);
        var punch = await Dex.GetMoveAsync(325, "frlg", Ct);
        Assert.Contains(punch!.Learners, l => l.SpeciesId == 94 && l.Method == "level-up");
    }

    [Fact]
    public async Task Gengar_existe_no_Lets_Go()
    {
        var g = await Dex.GetAsync(94, "lgpe", "en", Ct);
        Assert.True(g!.ExistsInGame);
        Assert.Contains(g.Forms, f => f.IsMega);
        Assert.NotEmpty(g.Moves.LevelUp);
        var gastly = await Dex.GetAsync(92, "lgpe", "en", Ct);
        Assert.Contains(gastly!.Encounters, e => e.Version is "lets-go-pikachu" or "lets-go-eevee");
    }

    [Fact]
    public async Task Sprigatito_nao_existe_no_FireRed()
    {
        var s = await Dex.GetAsync(906, "frlg", "pt-BR", Ct);
        Assert.NotNull(s);
        Assert.False(s.ExistsInGame);
        Assert.Empty(s.Moves.LevelUp);
        Assert.Empty(s.Moves.Machine);
        Assert.Empty(s.Encounters);
        Assert.Empty(s.FlavorTexts);
        Assert.Empty(s.Abilities);
    }

    [Fact]
    public async Task Especie_inexistente_da_null()
    {
        Assert.Null(await Dex.GetAsync(99999, "frlg", "en", Ct));
    }

    [Fact]
    public async Task Lista_filtra_por_texto_tipo_geracao_e_jogo()
    {
        var list = await Dex.ListAsync(new("geng", null, null, null), Ct);
        var gengar = Assert.Single(list);
        Assert.Equal(94, gengar.Id);
        Assert.Equal(94, gengar.NationalNumber);
        Assert.Equal(["ghost", "poison"], gengar.Types);

        Assert.Contains(await Dex.ListAsync(new("94", null, null, null), Ct), s => s.Id == 94);
        Assert.Contains(await Dex.ListAsync(new("flabebe", null, null, null), Ct), s => s.Id == 669);

        var ghosts = await Dex.ListAsync(new(null, "ghost", 1, null), Ct);
        Assert.Equal([92, 93, 94], ghosts.Select(s => s.Id));

        var frlg = await Dex.ListAsync(new(null, null, null, "frlg", 0, 2000), Ct);
        Assert.Contains(frlg, s => s.Id == 94);
        Assert.DoesNotContain(frlg, s => s.Id == 906);

        var page = await Dex.ListAsync(new(null, null, null, null, 10, 5), Ct);
        Assert.Equal([11, 12, 13, 14, 15], page.Select(s => s.Id));
    }

    [Fact]
    public async Task Lista_e_rapida()
    {
        await Dex.ListAsync(new(null, null, null, "frlg"), Ct); // aquece os caches
        var watch = Stopwatch.StartNew();
        await Dex.ListAsync(new("a", "fire", null, "sv", 0, 100), Ct);
        Assert.True(watch.ElapsedMilliseconds < 100, $"{watch.ElapsedMilliseconds} ms");
    }

    [Fact]
    public async Task Disponibilidade_por_jogo()
    {
        var all = await Dex.AvailabilityAsync(94, Ct);
        Assert.Equal(22, all.Count);
        Assert.Equal(AvailabilityKind.Evolution, all.Single(a => a.GameKey == "frlg").Kind);
        Assert.Equal(AvailabilityKind.None, (await Dex.AvailabilityAsync(906, Ct)).Single(a => a.GameKey == "frlg").Kind);

        var gastly = await Dex.AvailabilityAsync(92, Ct);
        Assert.Equal(AvailabilityKind.Encounter, gastly.Single(a => a.GameKey == "frlg").Kind);

        var pichu = await Dex.AvailabilityAsync(172, Ct);
        Assert.Equal(AvailabilityKind.None, pichu.Single(a => a.GameKey == "rb").Kind);
        Assert.Equal(AvailabilityKind.Egg, pichu.Single(a => a.GameKey == "frlg").Kind);
    }

    [Fact]
    public async Task Tabela_de_tipos_por_geracao()
    {
        var gen9 = await Dex.GetTypeChartAsync(9, Ct);
        Assert.Equal(18, gen9.Types.Count);
        Assert.Equal(2, gen9.Factors["fairy"]["dragon"]);
        var gen1 = await Dex.GetTypeChartAsync(1, Ct);
        Assert.Equal(15, gen1.Types.Count);
        Assert.DoesNotContain("steel", gen1.Types);
    }

    [Fact]
    public async Task Tabela_antiga_da_Gen_1()
    {
        var gen1 = (await Dex.GetTypeChartAsync(1, Ct)).Factors;
        Assert.Equal(0, gen1["ghost"]["psychic"]); // o bug da Gen 1
        Assert.Equal(2, gen1["bug"]["poison"]);
        Assert.Equal(2, gen1["poison"]["bug"]);
        Assert.Equal(1, gen1["ice"]["fire"]);
        var gen2 = (await Dex.GetTypeChartAsync(2, Ct)).Factors;
        Assert.Equal(2, gen2["ghost"]["psychic"]);
        Assert.Equal(0.5, gen2["bug"]["poison"]);
        Assert.Equal(0.5, gen2["ice"]["fire"]);
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(5)]
    public async Task Aco_resiste_a_Fantasma_e_Noturno_ate_a_Gen_5(int generation)
    {
        var chart = (await Dex.GetTypeChartAsync(generation, Ct)).Factors;
        Assert.Equal(0.5, chart["ghost"]["steel"]);
        Assert.Equal(0.5, chart["dark"]["steel"]);
        var gen6 = (await Dex.GetTypeChartAsync(6, Ct)).Factors;
        Assert.Equal(1, gen6["ghost"]["steel"]);
        Assert.Equal(1, gen6["dark"]["steel"]);
    }

    [Theory]
    [InlineData("gs", 0.5)]
    [InlineData("frlg", 0.5)]
    [InlineData("bw", 0.5)]
    [InlineData("sv", 1)]
    public async Task Steelix_contra_Fantasma(string game, double factor)
    {
        var steelix = await Dex.GetAsync(208, game, "en", Ct);
        Assert.Equal(factor, steelix!.DamageTaken["ghost"]);
        Assert.Equal(factor, steelix.DamageTaken["dark"]);
    }

    [Fact]
    public async Task Tipos_antigos_dos_Pokemon()
    {
        var clefairy = await Dex.GetAsync(35, "frlg", "en", Ct);
        Assert.Equal(["normal"], clefairy!.Types);
        Assert.Equal(["normal"], clefairy.Forms.Single(f => f.IsDefault).Types);
        Assert.Equal(2, clefairy.DamageTaken["fighting"]);
        Assert.Equal(0, clefairy.DamageTaken["ghost"]);
        Assert.Equal(["fairy"], (await Dex.GetAsync(35, "sv", "en", Ct))!.Types);

        Assert.Equal(["electric"], (await Dex.GetAsync(81, "rb", "en", Ct))!.Types);
        Assert.Equal(["electric", "steel"], (await Dex.GetAsync(81, "gs", "en", Ct))!.Types);

        var frlg = await Dex.ListAsync(new("clefairy", null, null, "frlg"), Ct);
        Assert.Equal(["normal"], Assert.Single(frlg).Types);
        Assert.Contains(await Dex.ListAsync(new(null, "normal", 1, "frlg", 0, 2000), Ct), s => s.Id == 35);
        Assert.Equal(["fairy"], Assert.Single(await Dex.ListAsync(new("clefairy", null, null, null), Ct)).Types);
    }

    [Fact]
    public async Task Pagina_da_especie_e_golpe_sao_rapidos()
    {
        await Dex.GetAsync(94, "sv", "en", Ct); // aquece os caches
        var watch = Stopwatch.StartNew();
        await Dex.GetAsync(94, "sv", "en", Ct);
        Assert.True(watch.ElapsedMilliseconds < 50, $"espécie: {watch.ElapsedMilliseconds} ms");
        await Dex.GetMoveAsync(33, "sv", Ct);
        watch.Restart();
        await Dex.GetMoveAsync(33, "sv", Ct);
        Assert.True(watch.ElapsedMilliseconds < 200, $"golpe: {watch.ElapsedMilliseconds} ms");
    }

    [Fact]
    public async Task Habilidade_e_item()
    {
        var levitate = await Dex.GetAbilityAsync(26, Ct);
        Assert.Equal("Levitate", levitate!.Name);
        Assert.Contains(levitate.Pokemon, p => p.SpeciesId == 92);
        var ball = await Dex.GetItemAsync(1, Ct);
        Assert.Equal("Master Ball", ball!.Name);
        Assert.Null(await Dex.GetItemAsync(999999, Ct));
    }

    [Fact]
    public void Copia_fica_com_o_hash_e_apaga_as_antigas()
    {
        Assert.SkipUnless(DexDatabase.ResourceExists, "Falta o dex.sqlite embutido.");
        var dir = Directory.CreateTempSubdirectory("traderock-dexcopy-").FullName;
        try
        {
            var folder = Path.Combine(dir, "dex");
            Directory.CreateDirectory(folder);
            File.WriteAllText(Path.Combine(folder, "00000000.sqlite"), "velho");
            var db = new DexDatabase(new AppPaths(dir));
            using (var c = db.Open())
            using (var cmd = c.CreateCommand())
            {
                cmd.CommandText = "SELECT count(*) FROM species";
                Assert.Equal(1025L, cmd.ExecuteScalar());
            }
            var file = Assert.Single(Directory.GetFiles(folder, "*.sqlite"));
            Assert.Matches("^[0-9a-f]{8}\\.sqlite$", Path.GetFileName(file));
            Assert.Equal(Path.GetFullPath(file), db.FilePath);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            try { Directory.Delete(dir, true); } catch (IOException) { }
        }
    }
}

public sealed class DexApiTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private readonly string _dir = Directory.CreateTempSubdirectory("traderock-dexapi-").FullName;
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
    public async Task Rotas_da_Pokedex()
    {
        Assert.SkipUnless(DexDatabase.ResourceExists, "Falta o dex.sqlite embutido.");
        var games = await _client.GetFromJsonAsync<JsonElement>("/api/dex/games");
        Assert.Equal("kanto", games.EnumerateArray().First(g => g.GetProperty("key").GetString() == "frlg")
            .GetProperty("region").GetString());

        var list = await _client.GetFromJsonAsync<JsonElement>("/api/dex/species?text=geng");
        Assert.Equal(94, list[0].GetProperty("id").GetInt32());

        var gengar = await _client.GetFromJsonAsync<JsonElement>("/api/dex/species/94?game=frlg&lang=pt-BR");
        Assert.True(gengar.GetProperty("existsInGame").GetBoolean());

        var availability = await _client.GetFromJsonAsync<JsonElement>("/api/dex/species/94/availability");
        Assert.Equal("evolution", availability.EnumerateArray().First(a => a.GetProperty("gameKey").GetString() == "frlg")
            .GetProperty("kind").GetString());

        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/api/dex/moves/33?game=frlg")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/api/dex/abilities/26")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/api/dex/items/1")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/api/dex/types?generation=3")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await _client.GetAsync("/api/dex/species/99999?game=frlg")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync("/api/dex/species/94?game=nada")).StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, (await _client.GetAsync("/api/dex/types?generation=10")).StatusCode);
    }
}
