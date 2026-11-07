using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using TradeRock.App;
using TradeRock.Core;
using TradeRock.Core.Builder;
using TradeRock.Core.Dex;
using TradeRock.Core.Trading;
using TradeRock.Core.Vgc;

namespace TradeRock.Tests;

public sealed class TeamAnalysisTests(DexFixture fx) : IClassFixture<DexFixture>
{
    private const string GengarBlastoise = """
        Gengar @ Life Orb
        Ability: Cursed Body
        Level: 50
        EVs: 4 HP / 252 SpA / 252 Spe
        Timid Nature
        - Shadow Ball
        - Sludge Bomb
        - Protect
        - Will-O-Wisp

        Blastoise @ Sitrus Berry
        Ability: Torrent
        Level: 50
        EVs: 252 HP / 252 SpA / 4 SpD
        Modest Nature
        - Water Spout
        - Ice Beam
        - Protect
        - Fake Out
        """;

    [Fact]
    public async Task Gengar_e_Blastoise_fraquezas_e_cobertura()
    {
        Assert.SkipUnless(DexDatabase.ResourceExists, "Falta o dex.sqlite embutido (rode scripts/build-dex.py).");
        var items = ShowdownImport.Parse(GengarBlastoise, new TrainerSpec("VGC", 0, 0), TradeGame.Scarlet);
        var team = items.Select(i => new TeamMember(i.Spec!.Species, i.Spec.Form, i.Spec.Moves)).ToList();
        var report = await TeamAnalysis.AnalyzeAsync(team, fx.Service, TestContext.Current.CancellationToken);

        Assert.Equal(["ghost", "poison"], report.Members[0].Types);
        Assert.Equal(["water"], report.Members[1].Types);
        var weak = report.Defense.Where(d => d.Weak > 0).Select(d => d.Type).ToHashSet();
        Assert.Superset(new HashSet<string> { "ghost", "dark", "psychic", "ground", "electric", "grass" }, weak);
        // Gengar é imune a Normal e Lutador; Blastoise resiste a Fogo, Água, Gelo e Aço.
        var normal = report.Defense.Single(d => d.Type == "normal");
        Assert.Equal(1, normal.Immune);
        Assert.Equal([0.0, 1.0], normal.Factors);
        Assert.Equal(1, report.Defense.Single(d => d.Type == "fire").Resist);

        // Golpes de status (Protect, Will-O-Wisp) não entram na cobertura; Fake Out (Normal) entra.
        Assert.Contains("ghost", report.AttackTypes);
        Assert.Contains("water", report.AttackTypes);
        Assert.Contains("normal", report.AttackTypes);
        Assert.DoesNotContain("fire", report.AttackTypes);
        var fire = report.Coverage.Single(c => c.Type == "fire");
        Assert.Equal(2.0, fire.Best);
        Assert.Contains("water", fire.SuperEffectiveBy);

        // Velocidades base, da mais rápida para a mais lenta.
        Assert.Equal([110, 78], report.Speeds.Select(s => s.BaseSpeed));
    }

    [Fact]
    public void Tipos_da_forma_vem_do_SV()
    {
        Assert.Equal(["electric", "water"], TeamAnalysis.TypesOf(479, 2)); // Wash Rotom
        Assert.Equal(["psychic", "normal"], TeamAnalysis.TypesOf(876, 1)); // Indeedee fêmea
        Assert.Empty(TeamAnalysis.TypesOf(25, 99));
    }
}

public sealed class VgcApiTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private readonly string _dir = Directory.CreateTempSubdirectory("traderock-vgcapi-").FullName;
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
    public async Task Times_regulamentacao_e_analise()
    {
        var teams = await _client.GetFromJsonAsync<JsonElement>("/api/vgc/teams");
        Assert.InRange(teams.GetArrayLength(), 6, 10);
        Assert.All(teams.EnumerateArray(), t => Assert.Equal(6, t.GetProperty("members").GetArrayLength()));

        var regulation = await _client.GetFromJsonAsync<JsonElement>("/api/vgc/regulation");
        Assert.Equal("Regulation Set M-C", regulation.GetProperty("name").GetString());
        Assert.Equal(4, regulation.GetProperty("pick").GetInt32());

        if (DexDatabase.ResourceExists)
        {
            var paste = teams[0].GetProperty("paste").GetString();
            var r = await _client.PostAsJsonAsync("/api/vgc/analyze", new { paste, game = "sv" });
            Assert.Equal(HttpStatusCode.OK, r.StatusCode);
            var body = await r.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal(6, body.GetProperty("report").GetProperty("members").GetArrayLength());
            Assert.Equal(18, body.GetProperty("report").GetProperty("defense").GetArrayLength());
            Assert.Equal(0, body.GetProperty("skipped").GetArrayLength());
        }

        await AssertError(new { paste = "isto não é um paste", game = "sv" }, "paste_invalid");
        await AssertError(new { paste = "", game = "sv" }, "paste_invalid");
        await AssertError(new { paste = new string('a', 20 * 1024 + 1), game = "sv" }, "paste_too_big");
        await AssertError(new { paste = "Pikachu\n- Thunderbolt", game = "frlg" }, "game_invalid");
    }

    private async Task AssertError(object body, string code)
    {
        var r = await _client.PostAsJsonAsync("/api/vgc/analyze", body);
        Assert.Equal(HttpStatusCode.BadRequest, r.StatusCode);
        var json = await r.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal(code, json.GetProperty("errors")[0].GetString());
    }
}
