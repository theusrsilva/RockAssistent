using System.Threading.Channels;
using TradeRock.App.Api;
using TradeRock.Core.Storage;

namespace TradeRock.App.Hotkeys;

/// <summary>
/// Liga as configurações de atalho da janela local (<c>hotkeys.global</c>) ao <see cref="IGlobalHotkeys"/> e repassa
/// as teclas apertadas para quem escuta <c>GET /api/hunts/events</c>. Só a janela do PC usa: as contas da rede não
/// ligam o atalho global (ele pegaria teclas do PC, não do celular).
/// </summary>
public sealed class GlobalHotkeyService : IHostedService
{
    public const string Increment = "increment";
    public const string Decrement = "decrement";

    private readonly IGlobalHotkeys _hotkeys;
    private readonly SettingsStore _settings;
    private readonly ILogger<GlobalHotkeyService> _logger;
    private readonly object _gate = new();
    private readonly List<Channel<string>> _subscribers = [];
    private string? _error;

    public GlobalHotkeyService(IGlobalHotkeys hotkeys, SettingsStore settings, ILogger<GlobalHotkeyService> logger)
    {
        _hotkeys = hotkeys;
        _settings = settings;
        _logger = logger;
        _hotkeys.Pressed += OnPressed;
    }

    public bool Supported => _hotkeys.Supported;

    /// <summary>Motivo do sistema (<c>wayland</c>, <c>no_x11</c>...) ou <c>in_use</c> se o último registro falhou.</summary>
    public string? Reason
    {
        get
        {
            lock (_gate)
                return _hotkeys.Reason ?? _error;
        }
    }

    /// <summary>
    /// Registra os dois atalhos quando <c>Global</c> está ligado, ou solta os dois. Em erro solta os dois e lança
    /// <see cref="GlobalHotkeyException"/>.
    /// </summary>
    public void Apply(SettingsEndpoints.HotkeySettings settings)
    {
        lock (_gate)
        {
            _hotkeys.Unregister(Increment);
            _hotkeys.Unregister(Decrement);
            _error = null;
            if (!settings.Global)
                return;
            try
            {
                _hotkeys.Register(Increment, settings.Increment);
                _hotkeys.Register(Decrement, settings.Decrement);
            }
            catch (GlobalHotkeyException ex)
            {
                _hotkeys.Unregister(Increment);
                _hotkeys.Unregister(Decrement);
                if (ex.Code == GlobalHotkeyException.InUse)
                    _error = "in_use";
                throw;
            }
        }
    }

    /// <summary>Teclas apertadas a partir de agora (até o Dispose).</summary>
    public Subscription Subscribe()
    {
        var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(64) { FullMode = BoundedChannelFullMode.DropOldest });
        lock (_subscribers)
            _subscribers.Add(channel);
        return new Subscription(channel.Reader, () =>
        {
            lock (_subscribers)
                _subscribers.Remove(channel);
            channel.Writer.TryComplete();
        });
    }

    public sealed class Subscription(ChannelReader<string> reader, Action dispose) : IDisposable
    {
        public ChannelReader<string> Reader { get; } = reader;
        public void Dispose() => dispose();
    }

    private void OnPressed(string id)
    {
        lock (_subscribers)
            foreach (var channel in _subscribers)
                channel.Writer.TryWrite(id);
    }

    /// <summary>Ao abrir o app: liga o atalho global se ficou ligado (configuração da janela local).</summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            SettingsEndpoints.HotkeySettings saved;
            using (OwnerScope.Use(OwnerScope.LocalOwner))
                saved = await _settings.GetAsync(SettingsEndpoints.HotkeysKey, SettingsEndpoints.HotkeySettings.Default, cancellationToken);
            if (saved.Global && _hotkeys.Supported)
                Apply(saved);
        }
        catch (GlobalHotkeyException ex)
        {
            _logger.LogWarning("Atalho global não ligou: {Code}.", ex.Code);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Atalho global não ligou.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            Apply(SettingsEndpoints.HotkeySettings.Default); // Global=false: solta as teclas
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Erro ao soltar o atalho global.");
        }
        return Task.CompletedTask;
    }
}
