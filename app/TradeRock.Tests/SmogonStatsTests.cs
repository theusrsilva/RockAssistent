using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using TradeRock.App;
using TradeRock.App.Server;
using TradeRock.Core;
using TradeRock.Core.Vgc;

namespace TradeRock.Tests;

/// <summary>Internet falsa: responde por URL e anota cada pedido (método, User-Agent, corpo).</summary>
internal sealed class FakeInternet : HttpMessageHandler
{
    public Dictionary<string, (HttpStatusCode Status, byte[] Body)> Routes { get; } = [];
    public List<string> Requests { get; } = [];

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var url = request.RequestUri!.ToString();
        lock (Requests)
            Requests.Add(url);
        // Só GET, sem corpo, com o User-Agent do app: nada do PC vai junto.
        Assert.Equal(HttpMethod.Get, request.Method);
        Assert.Null(request.Content);
        Assert.Contains("RockAssistent", request.Headers.UserAgent.ToString());
        var (status, body) = Routes.TryGetValue(url, out var r) ? r : (HttpStatusCode.NotFound, []);
        return Task.FromResult(new HttpResponseMessage(status) { Content = new ByteArrayContent(body) });
    }

    public HttpClient Client()
    {
        var http = new HttpClient(this, disposeHandler: false);
        InternetHttp.Configure(http, "1.0.0");
        return http;
    }
}

public sealed class SmogonStatsTests : IDisposable
{
    private static readonly string FixturePath = Path.Combine(AppContext.BaseDirectory, "Fixtures", "chaos-sample.json");
    private readonly string _dir = Directory.CreateTempSubdirectory("traderock-smogon-").FullName;
    private readonly FakeInternet _net = new();

    public void Dispose()
    {
        try { Directory.Delete(_dir, true); } catch (IOException) { }
    }

    private static UsageReport ParseFixture()
    {
        using var stream = File.OpenRead(FixturePath);
        return SmogonStats.Parse(stream, "2026-10");
    }

    [Fact]
    public void Parse_ordena_por_uso_e_normaliza_os_percentuais()
    {
        var report = ParseFixture();
        Assert.Equal("gen9championsvgc2026regmc-1760", report.Format);
        Assert.Equal("2026-10", report.Month);
        Assert.Equal(1200, report.Battles);
        Assert.Equal(["Sneasler", "Kingambit", "Rotom-Wash", "Swampert-Mega"], report.Pokemon.Select(p => p.Name));
        Assert.Equal([1, 2, 3, 4], report.Pokemon.Select(p => p.Rank));
        Assert.Equal(50, report.Pokemon[0].Usage);

        // Espécie pelo nome do Showdown, sem a forma.
        Assert.Equal(479, report.Pokemon[2].Species); // Rotom
        Assert.Equal(260, report.Pokemon[3].Species); // Swampert

        var sneasler = report.Pokemon[0];
        // Normalizado pelo peso (soma das habilidades): habilidades e itens somam 100%, golpes ~400%.
        Assert.InRange(sneasler.Abilities.Sum(a => a.Percent), 99.9, 100.1);
        Assert.InRange(sneasler.Items.Sum(a => a.Percent), 99.9, 100.1);
        Assert.InRange(sneasler.Moves.Sum(a => a.Percent), 318, 320);
        Assert.Equal(("closecombat", "Close Combat", 99.0), (sneasler.Moves[0].Id, sneasler.Moves[0].Name, sneasler.Moves[0].Percent));
        Assert.DoesNotContain(sneasler.Moves, m => m.Id == "");
        Assert.Equal(("unburden", "Unburden", 70.0), (sneasler.Abilities[0].Id, sneasler.Abilities[0].Name, sneasler.Abilities[0].Percent));
        Assert.Equal("White Herb", sneasler.Items[0].Name);
        Assert.Equal(["Kingambit", "Rotom-Wash", "Swampert-Mega"], sneasler.Teammates.Select(t => t.Name));
        Assert.Equal(15, sneasler.Teammates[0].Percent);

        var kingambit = report.Pokemon[1];
        Assert.Equal(["Kowtow Cleave", "Sucker Punch", "Protect", "Iron Head", "Swords Dance"], kingambit.Moves.Select(m => m.Name));
        Assert.Contains(kingambit.Items, i => i.Id == "nothing" && i.Percent == 10);
    }

    [Fact]
    public void Parse_recusa_arquivo_sem_data()
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("""{"info":{}}"""));
        Assert.Throws<InvalidDataException>(() => SmogonStats.Parse(stream));
    }

    [Fact]
    public void Formato_so_com_minusculas_digitos_e_corte()
    {
        Assert.True(SmogonStats.IsValidFormat(SmogonStats.DefaultFormat));
        Assert.True(SmogonStats.IsValidFormat("gen9championsvgc2026regmc"));
        Assert.False(SmogonStats.IsValidFormat("../etc/passwd"));
        Assert.False(SmogonStats.IsValidFormat("gen9vgc?x=1"));
        Assert.False(SmogonStats.IsValidFormat(null));
        Assert.Equal(new DateOnly(2026, 10, 1), SmogonStats.LatestMonth(new DateTimeOffset(2026, 11, 6, 3, 0, 0, TimeSpan.Zero)));
    }

    [Fact]
    public async Task Download_tenta_o_mes_anterior_no_404_e_grava_o_arquivo()
    {
        var format = SmogonStats.DefaultFormat;
        _net.Routes[SmogonStats.Url(format, new DateOnly(2026, 9, 1)).ToString()] = (HttpStatusCode.OK, File.ReadAllBytes(FixturePath));

        var done = await SmogonStats.DownloadAsync(_net.Client(), format, new DateOnly(2026, 10, 1), _dir, CancellationToken.None);

        Assert.Equal(new DateOnly(2026, 9, 1), done.Month);
        Assert.Equal(Path.Combine(_dir, $"{format}-2026-09.json"), done.File);
        Assert.True(File.Exists(done.File));
        Assert.Equal(
            [$"https://www.smogon.com/stats/2026-10/chaos/{format}.json", $"https://www.smogon.com/stats/2026-09/chaos/{format}.json"],
            _net.Requests);
        Assert.Equal("2026-09", SmogonStats.ParseFile(done.File).Month);
        Assert.Empty(Directory.GetFiles(_dir, "*.part"));
    }

    [Fact]
    public async Task Download_so_tenta_o_mes_anterior_uma_vez()
    {
        var e = await Assert.ThrowsAsync<InternetException>(() =>
            SmogonStats.DownloadAsync(_net.Client(), SmogonStats.DefaultFormat, new DateOnly(2026, 10, 1), _dir, CancellationToken.None));
        Assert.Equal("http_404", e.Code);
        Assert.Equal(2, _net.Requests.Count);
    }

    [Fact]
    public async Task Download_recusa_arquivo_que_nao_e_chaos_e_nao_deixa_lixo()
    {
        var format = SmogonStats.DefaultFormat;
        _net.Routes[SmogonStats.Url(format, new DateOnly(2026, 10, 1)).ToString()] = (HttpStatusCode.OK, "<html>oi</html>"u8.ToArray());
        var e = await Assert.ThrowsAsync<InternetException>(() =>
            SmogonStats.DownloadAsync(_net.Client(), format, new DateOnly(2026, 10, 1), _dir, CancellationToken.None));
        Assert.Equal("invalid_file", e.Code);
        Assert.Empty(Directory.GetFiles(_dir));
    }

    [Theory]
    [InlineData("https://evil.example/x")]
    [InlineData("http://pokepast.es/abc123")]
    [InlineData("https://pokepast.es.evil.example/abc123")]
    [InlineData("https://sub.pokepast.es/abc123")]
    [InlineData("https://pokepast.es:8443/abc123")]
    [InlineData("https://user@pokepast.es/abc123")]
    [InlineData("https://pokepast.es/")]
    [InlineData("https://pokepast.es/abc/def")]
    [InlineData("https://pokepast.es/abc-123")]
    public async Task PokePaste_recusa_outros_enderecos_sem_conectar(string url)
    {
        var e = await Assert.ThrowsAsync<InternetException>(() => PokePaste.FetchAsync(_net.Client(), new Uri(url), CancellationToken.None));
        Assert.Equal("paste_host_not_allowed", e.Code);
        Assert.Empty(_net.Requests);
    }

    [Fact]
    public async Task PokePaste_baixa_o_raw_com_limite()
    {
        Assert.Equal("https://pokepast.es/abc123/raw", PokePaste.RawUri("https://pokepast.es/abc123/raw")!.ToString());
        _net.Routes["https://pokepast.es/abc123/raw"] = (HttpStatusCode.OK, "Gengar @ Life Orb\r\n- Shadow Ball\r\n"u8.ToArray());
        Assert.Equal("Gengar @ Life Orb\n- Shadow Ball\n",
            await PokePaste.FetchAsync(_net.Client(), new Uri("https://POKEPAST.ES/abc123?x=1"), CancellationToken.None));

        _net.Routes["https://pokepast.es/big/raw"] = (HttpStatusCode.OK, new byte[VgcPaste.MaxBytes + 1]);
        var big = await Assert.ThrowsAsync<InternetException>(() =>
            PokePaste.FetchAsync(_net.Client(), new Uri("https://pokepast.es/big"), CancellationToken.None));
        Assert.Equal("paste_too_big", big.Code);

        var missing = await Assert.ThrowsAsync<InternetException>(() =>
            PokePaste.FetchAsync(_net.Client(), new Uri("https://pokepast.es/nope"), CancellationToken.None));
        Assert.Equal("paste_not_found", missing.Code);
    }

    [Theory]
    [InlineData("POST", "/api/vgc/usage/refresh", true)]
    [InlineData("POST", "/api/vgc/paste/fetch", true)]
    [InlineData("PUT", "/api/settings/internet", true)]
    [InlineData("GET", "/api/settings/internet", false)]
    [InlineData("GET", "/api/vgc/usage", false)]
    public void Downloads_so_pela_janela_do_PC(string method, string path, bool localOnly)
    {
        var context = new DefaultHttpContext();
        context.Request.Method = method;
        context.Request.Path = path;
        Assert.Equal(localOnly, AccessPolicy.IsLocalOnly(context.Request));
    }
}

public sealed class InternetApiTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private readonly string _dir = Directory.CreateTempSubdirectory("traderock-internet-").FullName;
    private readonly FakeInternet _net = new();
    private WebApplication? _app;
    private HttpClient _client = null!;

    public async ValueTask InitializeAsync()
    {
        _app = TradeRockServer.Create(["--data-dir", _dir], _token, b =>
        {
            b.WebHost.UseTestServer();
            b.Services.AddHttpClient(InternetHttp.ClientName).ConfigurePrimaryHttpMessageHandler(() => _net);
        });
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

    private static async Task<string?> ErrorOf(HttpResponseMessage response) =>
        (await response.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("error").GetString();

    [Fact]
    public async Task Desligado_por_padrao_nada_e_baixado()
    {
        var settings = await _client.GetFromJsonAsync<JsonElement>("/api/settings/internet");
        Assert.False(settings.GetProperty("smogon").GetBoolean());
        Assert.False(settings.GetProperty("pokepaste").GetBoolean());

        var refresh = await _client.PostAsync("/api/vgc/usage/refresh", null);
        Assert.Equal(HttpStatusCode.BadRequest, refresh.StatusCode);
        Assert.Equal("internet_disabled", await ErrorOf(refresh));

        var paste = await _client.PostAsJsonAsync("/api/vgc/paste/fetch", new { url = "https://pokepast.es/abc123" });
        Assert.Equal(HttpStatusCode.BadRequest, paste.StatusCode);
        Assert.Equal("internet_disabled", await ErrorOf(paste));

        var usage = await _client.GetAsync("/api/vgc/usage");
        Assert.Equal(HttpStatusCode.NotFound, usage.StatusCode);
        Assert.Equal("no_usage", await ErrorOf(usage));
        Assert.Empty(_net.Requests);
    }

    [Fact]
    public async Task Refresh_grava_o_arquivo_e_o_GET_devolve()
    {
        var put = await _client.PutAsJsonAsync("/api/settings/internet", new { smogon = true, pokepaste = false });
        put.EnsureSuccessStatusCode();

        var regulation = await _client.GetFromJsonAsync<JsonElement>("/api/vgc/regulation");
        var format = regulation.GetProperty("smogonFormat").GetString()!;
        var month = SmogonStats.LatestMonth(DateTimeOffset.UtcNow);
        _net.Routes[SmogonStats.Url(format, month).ToString()] =
            (HttpStatusCode.OK, File.ReadAllBytes(Path.Combine(AppContext.BaseDirectory, "Fixtures", "chaos-sample.json")));

        var refresh = await _client.PostAsync("/api/vgc/usage/refresh", null);
        refresh.EnsureSuccessStatusCode();
        Assert.True(File.Exists(Path.Combine(_dir, "vgc", SmogonStats.FileName(format, month))));

        var usage = await _client.GetFromJsonAsync<JsonElement>("/api/vgc/usage");
        Assert.Equal(format, usage.GetProperty("format").GetString());
        Assert.Equal(month.ToString("yyyy-MM"), usage.GetProperty("month").GetString());
        var first = usage.GetProperty("pokemon")[0];
        Assert.Equal("Sneasler", first.GetProperty("name").GetString());
        Assert.Equal(903, first.GetProperty("species").GetInt32());
        Assert.Equal("Close Combat", first.GetProperty("moves")[0].GetProperty("name").GetString());

        // Sem rede: 502 com um código curto.
        _net.Routes.Clear();
        _net.Routes[SmogonStats.Url(format, month).ToString()] = (HttpStatusCode.InternalServerError, []);
        var failed = await _client.PostAsync("/api/vgc/usage/refresh", null);
        Assert.Equal(HttpStatusCode.BadGateway, failed.StatusCode);
        var body = await failed.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Equal("network_error", body.GetProperty("error").GetString());
        Assert.Equal("http_500", body.GetProperty("reason").GetString());
        // O arquivo anterior continua valendo.
        Assert.Equal(HttpStatusCode.OK, (await _client.GetAsync("/api/vgc/usage")).StatusCode);
    }

    [Fact]
    public async Task Paste_so_do_pokepast_es()
    {
        (await _client.PutAsJsonAsync("/api/settings/internet", new { smogon = false, pokepaste = true })).EnsureSuccessStatusCode();
        _net.Routes["https://pokepast.es/abc123/raw"] = (HttpStatusCode.OK, "Gengar @ Life Orb\n- Shadow Ball\n"u8.ToArray());

        var evil = await _client.PostAsJsonAsync("/api/vgc/paste/fetch", new { url = "https://evil.example/x" });
        Assert.Equal(HttpStatusCode.BadRequest, evil.StatusCode);
        Assert.Equal("paste_host_not_allowed", await ErrorOf(evil));
        var plain = await _client.PostAsJsonAsync("/api/vgc/paste/fetch", new { url = "http://pokepast.es/abc123" });
        Assert.Equal("paste_host_not_allowed", await ErrorOf(plain));
        Assert.Empty(_net.Requests);

        var ok = await _client.PostAsJsonAsync("/api/vgc/paste/fetch", new { url = "https://pokepast.es/abc123" });
        ok.EnsureSuccessStatusCode();
        Assert.StartsWith("Gengar", (await ok.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("paste").GetString());

        // O Smogon continua desligado.
        Assert.Equal("internet_disabled", await ErrorOf(await _client.PostAsync("/api/vgc/usage/refresh", null)));
    }
}
