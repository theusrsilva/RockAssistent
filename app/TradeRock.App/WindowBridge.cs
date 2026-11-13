using System.Drawing;
using System.Text.Json;
using Photino.NET;
using TradeRock.Core.Storage;
using TradeRock.Core.Trading;

namespace TradeRock.App;

/// <summary>
/// Liga a janela nativa ao servidor. Fechar a janela durante uma troca não fecha na hora:
/// o frontend pergunta ao usuário e, se confirmado, chama /api/app/close, que cancela a troca
/// com segurança (encerra o motor e restaura o rádio) e só então fecha a janela.
/// </summary>
public sealed class WindowBridge(TradeCoordinator coordinator, SettingsStore settings, ILogger<WindowBridge> logger)
{
    /// <summary>Tamanho mínimo da janela normal.</summary>
    private static readonly Point NormalMinSize = new(900, 600);

    private readonly SemaphoreSlim _miniLock = new(1, 1);
    private PhotinoWindow? _window;
    private volatile bool _closeApproved;
    private volatile bool _huntOpen;
    private volatile bool _mini;
    private (Size Size, Point Location, bool Maximized) _before;

    /// <summary>A janela está encolhida como mini janela do Contador Hunt?</summary>
    public bool IsMini => _mini;

    /// <summary>O frontend avisa quando a tela de uma hunt em andamento abre ou fecha.</summary>
    public bool HuntOpen
    {
        get => _huntOpen;
        set => _huntOpen = value;
    }

    public bool HasWindow => _window is not null;

    public void Attach(PhotinoWindow window)
    {
        _window = window;
        window.RegisterWindowClosingHandler((_, _) =>
        {
            if (_closeApproved || !coordinator.IsBusy)
                return false; // pode fechar

            SendMessage(new { type = "close_requested" });
            return true; // segura a janela aberta até o usuário decidir
        });
        // Minimizou com uma hunt aberta: a janela volta encolhida e sempre na frente (se a opção estiver ligada). Fora
        // do tratador, para não mexer na janela no meio do aviso de minimizar.
        window.RegisterMinimizedHandler((_, _) =>
        {
            if (_huntOpen && !_mini)
                _ = Task.Run(() => MinimizedWithHuntAsync());
        });
    }

    private async Task MinimizedWithHuntAsync()
    {
        try
        {
            var saved = await settings.GetAsync(MiniWindowSettings.Key, new MiniWindowSettings(), CancellationToken.None);
            if (saved.OnMinimize)
                await SetMiniAsync(true, CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Não foi possível abrir a mini janela ao minimizar.");
        }
    }

    /// <summary>
    /// Liga ou desliga a mini janela: a própria janela encolhe, fica sempre na frente e vai para a última posição (ou o
    /// canto inferior direito); ao desligar, voltam o tamanho, a posição e o estado de antes, e a posição da mini janela
    /// fica guardada. Devolve o estado final; sem janela, false.
    /// </summary>
    public async Task<bool> SetMiniAsync(bool on, CancellationToken ct)
    {
        if (_window is not { } window)
            return false;
        await _miniLock.WaitAsync(ct);
        try
        {
            if (on == _mini)
            {
                if (on && window.Minimized)
                    window.Minimized = false;
                return _mini;
            }
            var saved = await settings.GetAsync(MiniWindowSettings.Key, new MiniWindowSettings(), ct);
            if (on)
            {
                if (window.Minimized)
                    window.Minimized = false;
                _before = (window.Size, window.Location, window.Maximized);
                if (window.Maximized)
                    window.Maximized = false;
                var size = MiniWindow.SizeFor(window.ScreenDpi, OperatingSystem.IsWindows());
                Point? last = saved is { Left: { } l, Top: { } t } ? new Point(l, t) : null;
                window.MinSize = new Point(size.Width, size.Height);
                window.Size = size;
                window.Location = MiniWindow.Place(window.MainMonitor.WorkArea, size, last);
                window.Topmost = true;
                _mini = true;
                logger.LogInformation("Mini janela do Contador Hunt ligada.");
            }
            else
            {
                var at = window.Location;
                window.Topmost = false;
                window.MinSize = NormalMinSize;
                window.Size = _before.Size;
                window.Location = _before.Location;
                if (_before.Maximized)
                    window.Maximized = true;
                _mini = false;
                await settings.SetAsync(MiniWindowSettings.Key, saved with { Left = at.X, Top = at.Y }, ct);
                logger.LogInformation("Mini janela do Contador Hunt desligada.");
            }
            SendMessage(new { type = "mini_window", on = _mini });
            return _mini;
        }
        finally
        {
            _miniLock.Release();
        }
    }

    /// <summary>Cancela a troca (se houver) e fecha a janela. Sem janela (--no-window), só cancela.</summary>
    public async Task CloseAsync()
    {
        await coordinator.CancelAsync(TimeSpan.FromSeconds(30));
        _closeApproved = true;
        if (_window is { } window)
        {
            logger.LogInformation("Fechando a janela a pedido do usuário.");
            window.Close();
        }
    }

    /// <summary>Janela nativa "abrir arquivo". Null se cancelado ou sem janela.</summary>
    public async Task<string?> PickOpenFileAsync(string title, (string Name, string[] Extensions)[] filters)
    {
        if (_window is not { } window)
            return null;
        var paths = await window.ShowOpenFileAsync(title, null, false, filters);
        return paths is { Length: > 0 } ? paths[0] : null;
    }

    /// <summary>Janela nativa "salvar arquivo". Null se cancelado ou sem janela.</summary>
    public async Task<string?> PickSaveFileAsync(string title, string suggestedName, (string Name, string[] Extensions)[] filters)
    {
        if (_window is not { } window)
            return null;
        var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        var path = await window.ShowSaveFileAsync(title, Path.Combine(documents, suggestedName), filters);
        return string.IsNullOrEmpty(path) ? null : path;
    }

    /// <summary>Janela nativa "escolher pasta". Null se cancelado ou sem janela.</summary>
    public async Task<string?> PickFolderAsync(string title)
    {
        if (_window is not { } window)
            return null;
        var paths = await window.ShowOpenFolderAsync(title);
        return paths is { Length: > 0 } ? paths[0] : null;
    }

    private void SendMessage(object message)
    {
        try
        {
            _window?.SendWebMessage(JsonSerializer.Serialize(message));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Não foi possível avisar o frontend.");
        }
    }
}
