using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TradeRock.App;
using TradeRock.App.Hotkeys;
using TradeRock.Core;
using Xunit;

namespace TradeRock.Tests;

/// <summary>Atalho global do Contador Hunt: combo, teclas, X11 sem tela e a API com um atalho falso.</summary>
public sealed class GlobalHotkeysTests
{
    [Fact]
    public void Le_o_combo_com_modificadores_vk_e_keysym()
    {
        var combo = HotkeyCombo.Parse("Ctrl+Shift+F7");
        Assert.Equal(HotkeyModifiers.Ctrl | HotkeyModifiers.Shift, combo.Modifiers);
        Assert.Equal("F7", combo.Code);
        Assert.Equal(0x2u | 0x4u, combo.WindowsModifiers); // MOD_CONTROL | MOD_SHIFT
        Assert.Equal(0x4u | 0x1u, combo.X11Mask); // ControlMask | ShiftMask
        Assert.True(HotkeyKeys.TryWindowsVk("F7", out var vk));
        Assert.Equal(0x76u, vk); // VK_F7
        Assert.True(HotkeyKeys.TryX11Keysym("F7", out var keysym));
        Assert.Equal(0xFFC4u, keysym); // XK_F7

        var meta = HotkeyCombo.Parse("Alt+Meta+KeyA");
        Assert.Equal(0x1u | 0x8u, meta.WindowsModifiers);
        Assert.Equal(0x8u | 0x40u, meta.X11Mask);
    }

    [Theory]
    [InlineData("Space", 0x20u, 0x20u)]
    [InlineData("KeyA", 0x41u, 0x61u)]
    [InlineData("Digit5", 0x35u, 0x35u)]
    [InlineData("Equal", 0xBBu, 0x3Du)]
    [InlineData("Minus", 0xBDu, 0x2Du)]
    [InlineData("ArrowUp", 0x26u, 0xFF52u)]
    [InlineData("Backspace", 0x08u, 0xFF08u)]
    [InlineData("Numpad3", 0x63u, 0xFFB3u)]
    [InlineData("NumpadAdd", 0x6Bu, 0xFFABu)]
    [InlineData("F1", 0x70u, 0xFFBEu)]
    [InlineData("F12", 0x7Bu, 0xFFC9u)]
    [InlineData("Delete", 0x2Eu, 0xFFFFu)]
    public void Tabela_de_teclas(string code, uint vk, uint keysym)
    {
        Assert.True(HotkeyKeys.TryWindowsVk(code, out var gotVk));
        Assert.Equal(vk, gotVk);
        Assert.True(HotkeyKeys.TryX11Keysym(code, out var gotKeysym));
        Assert.Equal(keysym, gotKeysym);
    }

    [Fact]
    public void Tecla_sem_mapeamento_ou_modificador_desconhecido_lanca_unsupported()
    {
        Assert.False(HotkeyKeys.TryWindowsVk("Semicolon", out _));
        Assert.Equal(GlobalHotkeyException.Unsupported, Assert.Throws<GlobalHotkeyException>(() => HotkeyCombo.Parse("Hyper+KeyA")).Code);
        var x11 = new X11Hotkeys(NullLogger<X11Hotkeys>.Instance, Env(("DISPLAY", ":0")), () => true);
        Assert.Equal(GlobalHotkeyException.Unsupported, Assert.Throws<GlobalHotkeyException>(() => x11.Register("increment", "Semicolon")).Code);
        x11.Dispose();
    }

    private static Func<string, string?> Env(params (string Key, string Value)[] values) =>
        key => values.FirstOrDefault(v => v.Key == key).Value;

    [Fact]
    public void X11_sem_tela_nao_tem_atalho_e_nao_explode()
    {
        Assert.Equal("no_x11", X11Hotkeys.Detect(Env(), () => true));
        Assert.Equal("wayland", X11Hotkeys.Detect(Env(("WAYLAND_DISPLAY", "wayland-0")), () => true));
        Assert.Equal("wayland", X11Hotkeys.Detect(Env(("DISPLAY", ":0"), ("XDG_SESSION_TYPE", "wayland")), () => true));
        Assert.Equal("no_x11", X11Hotkeys.Detect(Env(("DISPLAY", ":0")), () => false));
        Assert.Null(X11Hotkeys.Detect(Env(("DISPLAY", ":0"), ("XDG_SESSION_TYPE", "x11")), () => true));

        using var hotkeys = new X11Hotkeys(NullLogger<X11Hotkeys>.Instance, Env());
        Assert.False(hotkeys.Supported);
        Assert.Equal("no_x11", hotkeys.Reason);
        Assert.Equal(GlobalHotkeyException.GlobalUnsupported,
            Assert.Throws<GlobalHotkeyException>(() => hotkeys.Register("increment", "Space")).Code);
        hotkeys.Unregister("increment");
    }

    [Fact]
    public void Erro_na_thread_do_loop_chega_como_GlobalHotkeyException()
    {
        var failed = Task.Run(() => throw new GlobalHotkeyException(GlobalHotkeyException.InUse, "em uso"));
        Assert.Equal(GlobalHotkeyException.InUse,
            Assert.Throws<GlobalHotkeyException>(() => HotkeyThreadCall.Wait(failed, TimeSpan.FromSeconds(5))).Code);
        var stuck = new TaskCompletionSource().Task;
        Assert.Equal(GlobalHotkeyException.InUse,
            Assert.Throws<GlobalHotkeyException>(() => HotkeyThreadCall.Wait(stuck, TimeSpan.FromMilliseconds(50))).Code);
    }

    [Fact]
    public void Apply_com_erro_na_thread_solta_tudo_e_guarda_o_motivo()
    {
        var hotkeys = new ThreadedFakeHotkeys { Taken = "F9", NoKey = "Semicolon" };
        var service = new GlobalHotkeyService(hotkeys, null!, NullLogger<GlobalHotkeyService>.Instance);
        var previous = new TradeRock.App.Api.SettingsEndpoints.HotkeySettings("F7", "F8", true);
        service.Apply(previous);
        Assert.Equal("F7", hotkeys.Registered["increment"]);

        // O increment registra e o decrement falha: nada fica pego e o motivo vira in_use.
        var ex = Assert.Throws<GlobalHotkeyException>(() => service.Apply(new("F6", "F9", true)));
        Assert.Equal(GlobalHotkeyException.InUse, ex.Code);
        Assert.Empty(hotkeys.Registered);
        Assert.Equal("in_use", service.Reason);

        Assert.Equal(GlobalHotkeyException.Unsupported, Assert.Throws<GlobalHotkeyException>(() => service.Apply(new("Semicolon", "F8", true))).Code);
        Assert.Empty(hotkeys.Registered);

        service.Apply(previous); // o PUT volta o anterior
        Assert.Equal("F8", hotkeys.Registered["decrement"]);
        Assert.Null(service.Reason);
    }

    [Fact]
    public void NoHotkeys_avisa_o_motivo()
    {
        using var none = new NoHotkeys("unsupported_os");
        Assert.False(none.Supported);
        Assert.Equal("unsupported_os", none.Reason);
        Assert.Equal(GlobalHotkeyException.GlobalUnsupported, Assert.Throws<GlobalHotkeyException>(() => none.Register("increment", "Space")).Code);
        none.Unregister("increment");
    }
}

/// <summary>
/// Atalho falso que, como os de verdade, faz o trabalho numa thread própria e espera com <see cref="HotkeyThreadCall"/>:
/// o erro nasce na thread do loop.
/// </summary>
internal sealed class ThreadedFakeHotkeys : IGlobalHotkeys
{
    public string? Taken { get; set; }
    public string? NoKey { get; set; }
    public Dictionary<string, string> Registered { get; } = [];
    public bool Supported => true;
    public string? Reason => null;
#pragma warning disable CS0067
    public event Action<string>? Pressed;
#pragma warning restore CS0067

    private void OnThread(Action action)
    {
        var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        new Thread(() =>
        {
            try { action(); done.SetResult(); }
            catch (Exception ex) { done.SetException(ex); }
        }).Start();
        HotkeyThreadCall.Wait(done.Task, TimeSpan.FromSeconds(5));
    }

    public void Register(string id, string combo) => OnThread(() =>
    {
        if (combo == Taken)
            throw new GlobalHotkeyException(GlobalHotkeyException.InUse, "em uso");
        if (combo == NoKey)
            throw new GlobalHotkeyException(GlobalHotkeyException.Unsupported, "sem tecla");
        lock (Registered)
            Registered[id] = combo;
    });

    public void Unregister(string id) => OnThread(() =>
    {
        lock (Registered)
            Registered.Remove(id);
    });

    public void Dispose()
    {
    }
}

/// <summary>Atalho global falso: guarda o que foi registrado e aperta teclas quando o teste manda.</summary>
internal sealed class FakeGlobalHotkeys : IGlobalHotkeys
{
    public bool Supported { get; set; } = true;
    public string? Reason { get; set; }
    public HashSet<string> TakenCombos { get; } = [];
    public Dictionary<string, string> Registered { get; } = [];
    public event Action<string>? Pressed;

    public void Register(string id, string combo)
    {
        var parsed = HotkeyCombo.Parse(combo);
        if (!HotkeyKeys.TryWindowsVk(parsed.Code, out _))
            throw new GlobalHotkeyException(GlobalHotkeyException.Unsupported, "sem mapa");
        if (TakenCombos.Contains(combo))
            throw new GlobalHotkeyException(GlobalHotkeyException.InUse, "em uso");
        Registered[id] = combo;
    }

    public void Unregister(string id) => Registered.Remove(id);

    public void Press(string id) => Pressed?.Invoke(id);

    public void Dispose()
    {
    }
}

/// <summary>GET/PUT /api/settings/hotkeys com o atalho global e o SSE /api/hunts/events.</summary>
public sealed class GlobalHotkeysApiTests : IAsyncLifetime
{
    private readonly SessionToken _token = SessionToken.Generate();
    private readonly string _dir = Directory.CreateTempSubdirectory("traderock-globalkeys-").FullName;
    private readonly FakeGlobalHotkeys _fake = new();
    private WebApplication _app = null!;
    private HttpClient _client = null!;

    private static CancellationToken Ct => TestContext.Current.CancellationToken;

    public async ValueTask InitializeAsync()
    {
        _app = TradeRockServer.Create(["--data-dir", _dir], _token, b =>
        {
            b.WebHost.UseTestServer();
            b.Services.AddSingleton<IGlobalHotkeys>(_fake);
        });
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

    private async Task<(HttpStatusCode Status, JsonElement Body)> PutAsync(object body)
    {
        var response = await _client.PutAsJsonAsync("/api/settings/hotkeys", body, Ct);
        return (response.StatusCode, await response.Content.ReadFromJsonAsync<JsonElement>(Ct));
    }

    [Fact]
    public async Task Get_devolve_supported_e_o_motivo()
    {
        var initial = await _client.GetFromJsonAsync<JsonElement>("/api/settings/hotkeys", Ct);
        Assert.False(initial.GetProperty("global").GetBoolean());
        Assert.True(initial.GetProperty("supported").GetBoolean());
        Assert.Equal(JsonValueKind.Null, initial.GetProperty("reason").ValueKind);

        _fake.Supported = false;
        _fake.Reason = "wayland";
        var wayland = await _client.GetFromJsonAsync<JsonElement>("/api/settings/hotkeys", Ct);
        Assert.False(wayland.GetProperty("supported").GetBoolean());
        Assert.Equal("wayland", wayland.GetProperty("reason").GetString());
        var (status, body) = await PutAsync(new { increment = "Space", decrement = "Backspace", global = true });
        Assert.Equal(HttpStatusCode.Conflict, status);
        Assert.Equal("hotkey_global_unsupported", body.GetProperty("error").GetString());
        Assert.Empty(_fake.Registered);
    }

    [Fact]
    public async Task Ligar_registra_trocar_regista_de_novo_e_desligar_solta()
    {
        var (status, body) = await PutAsync(new { increment = "Ctrl+Shift+F7", decrement = "F8", global = true });
        Assert.Equal(HttpStatusCode.OK, status);
        Assert.True(body.GetProperty("global").GetBoolean());
        Assert.Equal("Ctrl+Shift+F7", _fake.Registered["increment"]);
        Assert.Equal("F8", _fake.Registered["decrement"]);

        // Tecla sem atalho global: 400 e nada muda.
        (status, body) = await PutAsync(new { increment = "Semicolon", decrement = "F8", global = true });
        Assert.Equal(HttpStatusCode.BadRequest, status);
        Assert.Equal("hotkey_unsupported", body.GetProperty("error").GetString());
        Assert.Equal("Ctrl+Shift+F7", _fake.Registered["increment"]);

        // Outro programa já usa: 409, volta o que valia.
        _fake.TakenCombos.Add("F9");
        (status, body) = await PutAsync(new { increment = "F9", decrement = "F8", global = true });
        Assert.Equal(HttpStatusCode.Conflict, status);
        Assert.Equal("hotkey_in_use", body.GetProperty("error").GetString());
        Assert.Equal("Ctrl+Shift+F7", _fake.Registered["increment"]);
        var saved = await _client.GetFromJsonAsync<JsonElement>("/api/settings/hotkeys", Ct);
        Assert.Equal("Ctrl+Shift+F7", saved.GetProperty("increment").GetString());

        (status, _) = await PutAsync(new { increment = "Ctrl+Shift+F7", decrement = "F8", global = false });
        Assert.Equal(HttpStatusCode.OK, status);
        Assert.Empty(_fake.Registered);
        Assert.False((await _client.GetFromJsonAsync<JsonElement>("/api/settings/hotkeys", Ct)).GetProperty("global").GetBoolean());
    }

    [Fact]
    public async Task Eventos_mandam_a_tecla_apertada()
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(Ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(20));
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/hunts/events");
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/event-stream"));
        using var response = await _client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeout.Token);
        Assert.Equal("text/event-stream", response.Content.Headers.ContentType?.MediaType);

        await using var stream = await response.Content.ReadAsStreamAsync(timeout.Token);
        using var reader = new StreamReader(stream);
        Assert.Equal(": ok", await reader.ReadLineAsync(timeout.Token)); // inscrito

        _fake.Press("increment");
        _fake.Press("decrement");
        var events = new List<JsonElement>();
        while (events.Count < 2 && await reader.ReadLineAsync(timeout.Token) is { } line)
            if (line.StartsWith("data: ", StringComparison.Ordinal))
                events.Add(JsonDocument.Parse(line["data: ".Length..]).RootElement.Clone());
        Assert.Equal("hotkey", events[0].GetProperty("type").GetString());
        Assert.Equal("increment", events[0].GetProperty("id").GetString());
        Assert.Equal("decrement", events[1].GetProperty("id").GetString());
    }
}
