using System.Threading.Channels;
using TradeRock.App.Api;
using TradeRock.Core.Storage;

namespace TradeRock.App.Hotkeys;

/// <summary>
/// Liga as configurações de atalho da janela local (<c>hotkeys.global</c>) ao <see cref="IGlobalHotkeys"/> e repassa
/// as teclas apertadas para quem escuta <c>GET /api/hunts/events</c>. Só a janela do PC usa: as contas da rede não
/// ligam o atalho global (ele pegaria teclas do PC, não do celular).
/// A opção ligada não pega tecla nenhuma por si só: o sistema só fica com as teclas enquanto houver um contador
/// aberto escutando os eventos (o primeiro assinante registra, o último a sair solta).
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
    private SettingsEndpoints.HotkeySettings _desired = SettingsEndpoints.HotkeySettings.Default;
    private int _listeners;
    private bool _active;
    private bool _stopped;
    private string? _error;

    public GlobalHotkeyService(IGlobalHotkeys hotkeys, SettingsStore settings, ILogger<GlobalHotkeyService> logger)
    {
        _hotkeys = hotkeys;
        _settings = settings;
        _logger = logger;
        _hotkeys.Pressed += OnPressed;
    }

    public bool Supported => _hotkeys.Supported;

    /// <summary>As teclas estão pegas agora (opção ligada e um contador aberto).</summary>
    public bool Active
    {
        get
        {
            lock (_gate)
                return _active;
        }
    }

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
    /// Guarda a configuração. Com <c>Global</c> ligado confere se as teclas podem ser pegas: com um contador aberto
    /// registra de vez; sem ninguém escutando registra e solta na hora (só para avisar do erro). Desligado, solta.
    /// Em erro solta os dois e lança <see cref="GlobalHotkeyException"/>.
    /// </summary>
    public void Apply(SettingsEndpoints.HotkeySettings settings)
    {
        lock (_gate)
        {
            _desired = settings;
            ReleaseLocked();
            _error = null;
            if (!settings.Global)
                return;
            RegisterLocked(settings);
            if (_listeners == 0 || _stopped)
                ReleaseLocked();
        }
    }

    private void RegisterLocked(SettingsEndpoints.HotkeySettings settings)
    {
        try
        {
            _hotkeys.Register(Increment, settings.Increment);
            _hotkeys.Register(Decrement, settings.Decrement);
            _active = true;
        }
        catch (GlobalHotkeyException ex)
        {
            ReleaseLocked();
            if (ex.Code == GlobalHotkeyException.InUse)
                _error = "in_use";
            throw;
        }
    }

    private void ReleaseLocked()
    {
        _hotkeys.Unregister(Increment);
        _hotkeys.Unregister(Decrement);
        _active = false;
    }

    /// <summary>Teclas apertadas a partir de agora (até o Dispose). O primeiro assinante pega as teclas.</summary>
    public Subscription Subscribe()
    {
        var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(64) { FullMode = BoundedChannelFullMode.DropOldest });
        lock (_subscribers)
            _subscribers.Add(channel);
        lock (_gate)
        {
            _listeners++;
            if (_listeners == 1 && !_active && !_stopped && _desired.Global && _hotkeys.Supported
                && !SettingsEndpoints.IsTextKey(_desired.Increment) && !SettingsEndpoints.IsTextKey(_desired.Decrement))
            {
                try
                {
                    RegisterLocked(_desired);
                    _error = null;
                }
                catch (GlobalHotkeyException ex)
                {
                    _logger.LogWarning("Atalho global não ligou: {Code}.", ex.Code);
                }
            }
        }
        var disposed = 0;
        return new Subscription(channel.Reader, () =>
        {
            if (Interlocked.Exchange(ref disposed, 1) == 1)
                return;
            lock (_subscribers)
                _subscribers.Remove(channel);
            channel.Writer.TryComplete();
            lock (_gate)
            {
                _listeners--;
                if (_listeners == 0 && _active)
                    ReleaseLocked();
            }
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

    /// <summary>Ao abrir o app: lê a configuração da janela local. Não pega tecla nenhuma até um contador abrir.</summary>
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            SettingsEndpoints.HotkeySettings saved;
            using (OwnerScope.Use(OwnerScope.LocalOwner))
                saved = await _settings.GetAsync(SettingsEndpoints.HotkeysKey, SettingsEndpoints.HotkeySettings.Default, cancellationToken);
            lock (_gate)
                _desired = saved;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogWarning(ex, "Configuração do atalho global não foi lida.");
        }
    }

    /// <summary>Ao fechar: solta tudo e não pega mais.</summary>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        try
        {
            lock (_gate)
            {
                _stopped = true;
                ReleaseLocked();
            }
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Erro ao soltar o atalho global.");
        }
        return Task.CompletedTask;
    }
}
