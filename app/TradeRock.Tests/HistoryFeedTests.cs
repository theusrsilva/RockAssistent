using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TradeRock.App;
using TradeRock.Core;
using TradeRock.Core.Storage;
using TradeRock.Core.Trading;

namespace TradeRock.Tests;

/// <summary>Histórico unificado: trocas, hunts terminadas e runs terminadas, por dono.</summary>
public sealed class HistoryFeedTests(DexFixture fx) : IClassFixture<DexFixture>, IAsyncLifetime
{
    private readonly TestDataDir _dir = new();
    private IDbContextFactory<TradeRockDb> _db = null!;
    private HistoryFeed _feed = null!;
    private static readonly DateTimeOffset T0 = new(2026, 11, 1, 12, 0, 0, TimeSpan.Zero);

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        _db = await _dir.CreateDbFactoryAsync();
        _feed = new HistoryFeed(_db, fx.Service);
    }

    public ValueTask DisposeAsync()
    {
        SqliteConnection.ClearAllPools();
        _dir.Dispose();
        return ValueTask.CompletedTask;
    }

    private async Task SeedAsync()
    {
        await using var db = await _db.CreateDbContextAsync(Ct);
        db.Trades.Add(new TradeRecord
        {
            Id = Guid.NewGuid(), StartedAt = T0, EndedAt = T0.AddMinutes(5), EngineId = "fake", Game = TradeGame.Sword,
            TrainerName = "Ash", Status = TradeSessionStatus.Completed, TradesPlanned = 1, TradesCompleted = 1, EventsJson = "[]",
        });
        db.Hunts.Add(new ShinyHunt
        {
            Id = Guid.NewGuid(), Species = 906, GameKey = "sv", MethodId = "masuda", ModifiersJson = "{}",
            StartedAt = T0, EndedAt = T0.AddDays(2), Found = true,
        });
        // Hunt em andamento: não entra.
        db.Hunts.Add(new ShinyHunt
        {
            Id = Guid.NewGuid(), Species = 25, GameKey = "sv", MethodId = "masuda", ModifiersJson = "{}", StartedAt = T0,
        });
        db.NuzlockeRuns.Add(new NuzlockeRun
        {
            Id = Guid.NewGuid(), Name = "Run 1", GameKey = "frlg", RulesJson = "{}", Status = "wipe",
            StartedAt = T0.AddDays(-3), EndedAt = T0.AddDays(1),
        });
        // Run ativa: não entra.
        db.NuzlockeRuns.Add(new NuzlockeRun
        {
            Id = Guid.NewGuid(), Name = "Run 2", GameKey = "frlg", RulesJson = "{}", Status = "active", StartedAt = T0,
        });
        await db.SaveChangesAsync(Ct);
    }

    [Fact]
    public async Task Junta_tudo_do_mais_novo_para_o_mais_velho()
    {
        await SeedAsync();
        var items = await _feed.ListAsync(null, HistoryFeed.DefaultLimit, Ct);

        Assert.Equal(["hunt", "nuzlocke", "trade"], items.Select(i => i.Kind));
        var hunt = items[0];
        Assert.Equal("Sprigatito", hunt.Title);
        Assert.Equal(906, hunt.Species);
        Assert.True(hunt.Ok);
        Assert.Equal("sv", hunt.GameKey);
        Assert.False(items[1].Ok); // wipe
        Assert.Equal("Run 1", items[1].Title);
        Assert.Equal("swsh", items[2].GameKey);
        Assert.Equal("completed", items[2].Subtitle);
        Assert.True(items[2].Ok);
    }

    [Fact]
    public async Task Filtra_por_tipo_e_respeita_o_limite()
    {
        await SeedAsync();
        Assert.Equal(["hunt"], (await _feed.ListAsync("hunt", 50, Ct)).Select(i => i.Kind));
        Assert.Empty(await _feed.ListAsync("raid", 50, Ct));
        Assert.Equal(["hunt"], (await _feed.ListAsync(null, 1, Ct)).Select(i => i.Kind));
        await Assert.ThrowsAsync<ArgumentException>(() => _feed.ListAsync("x", 50, Ct));
    }

    [Fact]
    public async Task Dono_diferente_nao_aparece()
    {
        await SeedAsync();
        using (OwnerScope.Use(Guid.NewGuid()))
            Assert.Empty(await _feed.ListAsync(null, 50, Ct));
        Assert.Equal(3, (await _feed.ListAsync(null, 50, Ct)).Count);
    }
}

/// <summary>GET /api/history pela API.</summary>
public sealed class HistoryApiTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private WebApplication _app = null!;
    private string _dataDir = null!;
    private HttpClient _client = null!;

    public async ValueTask InitializeAsync()
    {
        _dataDir = Directory.CreateTempSubdirectory("traderock-history-").FullName;
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
        try { Directory.Delete(_dataDir, recursive: true); } catch (IOException) { }
    }

    [Fact]
    public async Task Tipo_invalido_da_400()
    {
        var ct = TestContext.Current.CancellationToken;
        var bad = await _client.GetAsync("/api/history?kind=foo", ct);
        Assert.Equal(HttpStatusCode.BadRequest, bad.StatusCode);
        var body = await bad.Content.ReadFromJsonAsync<JsonElement>(ct);
        Assert.Equal("invalid_kind", body.GetProperty("error").GetString());

        var ok = await _client.GetAsync("/api/history?kind=hunt&limit=10", ct);
        Assert.Equal(HttpStatusCode.OK, ok.StatusCode);
        Assert.Equal(JsonValueKind.Array, (await ok.Content.ReadFromJsonAsync<JsonElement>(ct)).ValueKind);
    }
}
