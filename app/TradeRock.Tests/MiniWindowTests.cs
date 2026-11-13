using System.Drawing;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using TradeRock.App;
using TradeRock.Core;

namespace TradeRock.Tests;

/// <summary>Mini janela do Contador Hunt (D40): as contas de tamanho e posição, sem a janela nativa.</summary>
public sealed class MiniWindowTests
{
    private static readonly Rectangle Work = new(0, 0, 1920, 1040);

    [Fact]
    public void Sem_posicao_guardada_abre_no_canto_inferior_direito()
    {
        var size = MiniWindow.BaseSize;
        var at = MiniWindow.Place(Work, size, null);
        Assert.Equal(new Point(1920 - size.Width - MiniWindow.Margin, 1040 - size.Height - MiniWindow.Margin), at);
    }

    [Fact]
    public void Lembra_a_ultima_posicao_se_ainda_cabe_na_tela()
    {
        Assert.Equal(new Point(100, 200), MiniWindow.Place(Work, MiniWindow.BaseSize, new Point(100, 200)));
        // Monitor que saiu ou resolução menor: volta para o canto.
        var corner = MiniWindow.Place(Work, MiniWindow.BaseSize, null);
        Assert.Equal(corner, MiniWindow.Place(Work, MiniWindow.BaseSize, new Point(3000, 200)));
        Assert.Equal(corner, MiniWindow.Place(Work, MiniWindow.BaseSize, new Point(1900, 1000)));
    }

    [Fact]
    public void Tela_menor_que_a_mini_janela_nao_sai_da_area_de_trabalho()
    {
        var at = MiniWindow.Place(new Rectangle(10, 20, 200, 100), MiniWindow.BaseSize, null);
        Assert.Equal(new Point(10, 20), at);
    }

    [Theory]
    [InlineData(96u, true, 340, 230)]
    [InlineData(144u, true, 510, 345)]
    [InlineData(144u, false, 340, 230)]
    [InlineData(0u, true, 340, 230)]
    public void Tamanho_acompanha_a_escala_so_no_Windows(uint dpi, bool windows, int width, int height) =>
        Assert.Equal(new Size(width, height), MiniWindow.SizeFor(dpi, windows));
}

/// <summary>GET/PUT /api/app/mini-window sem janela nativa (como nos testes e no navegador).</summary>
public sealed class MiniWindowApiTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private readonly string _dir = Directory.CreateTempSubdirectory("traderock-mini-").FullName;
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

    [Fact]
    public async Task Sem_janela_nao_ha_mini_janela_e_a_opcao_comeca_ligada()
    {
        var state = await _client.GetFromJsonAsync<JsonElement>("/api/app/mini-window", Ct);
        Assert.False(state.GetProperty("supported").GetBoolean());
        Assert.False(state.GetProperty("on").GetBoolean());
        Assert.True(state.GetProperty("onMinimize").GetBoolean());

        var response = await _client.PutAsJsonAsync("/api/app/mini-window", new { on = true }, Ct);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
