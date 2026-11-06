using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using TradeRock.App;
using TradeRock.App.Api;
using TradeRock.Core;
using Xunit;

namespace TradeRock.Tests;

/// <summary>GET/PUT /api/settings/hotkeys (atalhos do Contador Hunt) na janela local.</summary>
public sealed class HotkeySettingsTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private readonly string _dir = Directory.CreateTempSubdirectory("traderock-hotkeys-").FullName;
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

    private async Task<string> ErrorOf(HttpResponseMessage response)
    {
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>(Ct);
        return body.GetProperty("error").GetString()!;
    }

    [Fact]
    public async Task Padrao_e_espaco_e_backspace_e_salva_o_novo()
    {
        var initial = await _client.GetFromJsonAsync<JsonElement>("/api/settings/hotkeys", Ct);
        Assert.Equal("Space", initial.GetProperty("increment").GetString());
        Assert.Equal("Backspace", initial.GetProperty("decrement").GetString());

        var put = await _client.PutAsJsonAsync("/api/settings/hotkeys", new { increment = "Shift+Equal", decrement = "Minus" }, Ct);
        Assert.Equal(HttpStatusCode.OK, put.StatusCode);
        var saved = await _client.GetFromJsonAsync<JsonElement>("/api/settings/hotkeys", Ct);
        Assert.Equal("Shift+Equal", saved.GetProperty("increment").GetString());
        Assert.Equal("Minus", saved.GetProperty("decrement").GetString());
    }

    [Fact]
    public async Task Recusa_atalhos_iguais_ou_invalidos()
    {
        Assert.Equal("hotkeys_same", await ErrorOf(await _client.PutAsJsonAsync("/api/settings/hotkeys",
            new { increment = "Space", decrement = "space" }, Ct)));
        Assert.Equal("hotkey_invalid", await ErrorOf(await _client.PutAsJsonAsync("/api/settings/hotkeys",
            new { increment = "", decrement = "Backspace" }, Ct)));
        Assert.Equal("hotkey_invalid", await ErrorOf(await _client.PutAsJsonAsync("/api/settings/hotkeys",
            new { increment = "Ctrl+Shift", decrement = "Backspace" }, Ct)));
        Assert.Equal("hotkey_invalid", await ErrorOf(await _client.PutAsJsonAsync("/api/settings/hotkeys",
            new { increment = "Ctrl+<script>", decrement = "Backspace" }, Ct)));

        // Nada mudou.
        var current = await _client.GetFromJsonAsync<JsonElement>("/api/settings/hotkeys", Ct);
        Assert.Equal("Space", current.GetProperty("increment").GetString());
    }

    [Theory]
    [InlineData("Space", true)]
    [InlineData("Ctrl+ArrowUp", true)]
    [InlineData("Ctrl+Alt+Shift+Meta+KeyA", true)]
    [InlineData("Shift", false)]
    [InlineData("Ctrl+", false)]
    [InlineData("Foo+KeyA", false)]
    [InlineData("Tab", false)]
    [InlineData("Shift+Tab", false)]
    [InlineData("Enter", false)]
    [InlineData("Ctrl+Escape", false)]
    public void Valida_o_combo(string combo, bool valid) => Assert.Equal(valid, SettingsEndpoints.IsValidCombo(combo));
}
