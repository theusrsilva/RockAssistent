namespace TradeRock.App.Hotkeys;

/// <summary>Sem atalho global (Wayland, sem X11, macOS...): só o atalho da janela vale.</summary>
public sealed class NoHotkeys(string reason) : IGlobalHotkeys
{
    public bool Supported => false;
    public string? Reason { get; } = reason;

#pragma warning disable CS0067 // nunca dispara
    public event Action<string>? Pressed;
#pragma warning restore CS0067

    public void Register(string id, string combo) =>
        throw new GlobalHotkeyException(GlobalHotkeyException.GlobalUnsupported, $"Atalho global indisponível: {Reason}");

    public void Unregister(string id)
    {
    }

    public void Dispose()
    {
    }
}
