namespace TradeRock.App.Hotkeys;

/// <summary>
/// Atalho global do Contador Hunt: a tecla vale com o app em segundo plano (quem joga no emulador). Windows pelo
/// <c>RegisterHotKey</c>, Linux com X11 pelo <c>XGrabKey</c>; Wayland e os outros casos não têm (<see cref="NoHotkeys"/>).
/// O combo é o mesmo dos atalhos da janela: <c>KeyboardEvent.code</c> com os modificadores na frente
/// (<c>Space</c>, <c>Ctrl+Shift+F7</c>).
/// </summary>
public interface IGlobalHotkeys : IDisposable
{
    /// <summary>O sistema deixa pegar teclas globais.</summary>
    bool Supported { get; }

    /// <summary>Por que não (<c>wayland</c>, <c>no_x11</c>, <c>unsupported_os</c>); nulo quando funciona.</summary>
    string? Reason { get; }

    /// <summary>
    /// Registra (ou troca) o atalho <paramref name="id"/>. Lança <see cref="GlobalHotkeyException"/>:
    /// <c>hotkey_unsupported</c> (tecla sem mapeamento), <c>hotkey_in_use</c> (outro programa já pegou a tecla) ou
    /// <c>hotkey_global_unsupported</c> (o sistema não deixa).
    /// </summary>
    void Register(string id, string combo);

    /// <summary>Solta o atalho <paramref name="id"/> (sem erro se não existe).</summary>
    void Unregister(string id);

    /// <summary>Atalho apertado (o id do <see cref="Register"/>). Chega de outra thread.</summary>
    event Action<string>? Pressed;
}

public sealed class GlobalHotkeyException(string code, string message) : Exception(message)
{
    public const string Unsupported = "hotkey_unsupported";
    public const string InUse = "hotkey_in_use";
    public const string GlobalUnsupported = "hotkey_global_unsupported";

    public string Code { get; } = code;
}

[Flags]
public enum HotkeyModifiers
{
    None = 0,
    Ctrl = 1,
    Alt = 2,
    Shift = 4,
    Meta = 8,
}

/// <summary>Combo lido: modificadores e o código da tecla (<c>KeyboardEvent.code</c>).</summary>
public readonly record struct HotkeyCombo(HotkeyModifiers Modifiers, string Code)
{
    /// <summary>"Ctrl+Shift+F7" → Ctrl|Shift e "F7". Lança <c>hotkey_unsupported</c> se o texto não é um combo.</summary>
    public static HotkeyCombo Parse(string combo)
    {
        var parts = (combo ?? "").Split('+', StringSplitOptions.TrimEntries);
        var code = parts[^1];
        var mods = HotkeyModifiers.None;
        foreach (var part in parts[..^1])
        {
            mods |= part.ToLowerInvariant() switch
            {
                "ctrl" or "control" => HotkeyModifiers.Ctrl,
                "alt" => HotkeyModifiers.Alt,
                "shift" => HotkeyModifiers.Shift,
                "meta" => HotkeyModifiers.Meta,
                _ => throw new GlobalHotkeyException(GlobalHotkeyException.Unsupported, $"Modificador desconhecido: {part}"),
            };
        }
        if (code.Length == 0)
            throw new GlobalHotkeyException(GlobalHotkeyException.Unsupported, "Combo sem tecla.");
        return new HotkeyCombo(mods, code);
    }

    /// <summary>Modificadores do <c>RegisterHotKey</c> (MOD_ALT 1, MOD_CONTROL 2, MOD_SHIFT 4, MOD_WIN 8).</summary>
    public uint WindowsModifiers =>
        (Modifiers.HasFlag(HotkeyModifiers.Alt) ? 0x1u : 0)
        | (Modifiers.HasFlag(HotkeyModifiers.Ctrl) ? 0x2u : 0)
        | (Modifiers.HasFlag(HotkeyModifiers.Shift) ? 0x4u : 0)
        | (Modifiers.HasFlag(HotkeyModifiers.Meta) ? 0x8u : 0);

    /// <summary>Máscara do X11 (ShiftMask 1, ControlMask 4, Mod1Mask 8 = Alt, Mod4Mask 64 = Super).</summary>
    public uint X11Mask =>
        (Modifiers.HasFlag(HotkeyModifiers.Shift) ? 0x1u : 0)
        | (Modifiers.HasFlag(HotkeyModifiers.Ctrl) ? 0x4u : 0)
        | (Modifiers.HasFlag(HotkeyModifiers.Alt) ? 0x8u : 0)
        | (Modifiers.HasFlag(HotkeyModifiers.Meta) ? 0x40u : 0);
}

/// <summary>
/// Tabela <c>KeyboardEvent.code</c> → virtual-key do Windows e keysym do X11, para as teclas usuais. Os dois sistemas
/// pensam na tecla do layout (o <c>code</c> é a posição física): num teclado AZERTY, "KeyA" vira a tecla que escreve "a".
/// </summary>
public static class HotkeyKeys
{
    private static readonly Dictionary<string, (uint Vk, uint Keysym)> Table = Build();

    private static Dictionary<string, (uint, uint)> Build()
    {
        var t = new Dictionary<string, (uint, uint)>(StringComparer.Ordinal);
        for (var c = 'A'; c <= 'Z'; c++)
            t[$"Key{c}"] = (c, char.ToLowerInvariant(c)); // VK 0x41..; keysym a..z (0x61..)
        for (var d = 0; d <= 9; d++)
        {
            t[$"Digit{d}"] = ((uint)('0' + d), (uint)('0' + d)); // VK 0x30..; keysym 0..9
            t[$"Numpad{d}"] = (0x60u + (uint)d, 0xFFB0u + (uint)d); // VK_NUMPAD0..; XK_KP_0..
        }
        for (var f = 1; f <= 12; f++)
            t[$"F{f}"] = (0x6Fu + (uint)f, 0xFFBDu + (uint)f); // VK_F1 0x70..; XK_F1 0xFFBE..
        t["ArrowLeft"] = (0x25, 0xFF51);
        t["ArrowUp"] = (0x26, 0xFF52);
        t["ArrowRight"] = (0x27, 0xFF53);
        t["ArrowDown"] = (0x28, 0xFF54);
        t["Space"] = (0x20, 0x0020);
        t["Backspace"] = (0x08, 0xFF08);
        t["Equal"] = (0xBB, 0x003D); // VK_OEM_PLUS; XK_equal
        t["Minus"] = (0xBD, 0x002D); // VK_OEM_MINUS; XK_minus
        t["NumpadAdd"] = (0x6B, 0xFFAB);
        t["NumpadSubtract"] = (0x6D, 0xFFAD);
        t["NumpadMultiply"] = (0x6A, 0xFFAA);
        t["NumpadDivide"] = (0x6F, 0xFFAF);
        t["NumpadDecimal"] = (0x6E, 0xFFAE);
        t["PageUp"] = (0x21, 0xFF55); // XK_Prior
        t["PageDown"] = (0x22, 0xFF56); // XK_Next
        t["End"] = (0x23, 0xFF57);
        t["Home"] = (0x24, 0xFF50);
        t["Insert"] = (0x2D, 0xFF63);
        t["Delete"] = (0x2E, 0xFFFF);
        return t;
    }

    public static bool TryWindowsVk(string code, out uint vk)
    {
        var ok = Table.TryGetValue(code, out var entry);
        vk = entry.Vk;
        return ok;
    }

    public static bool TryX11Keysym(string code, out uint keysym)
    {
        var ok = Table.TryGetValue(code, out var entry);
        keysym = entry.Keysym;
        return ok;
    }

    internal static GlobalHotkeyException Unmapped(string code) =>
        new(GlobalHotkeyException.Unsupported, $"Tecla sem atalho global: {code}");
}

/// <summary>Espera o pedido que roda na thread dos atalhos.</summary>
internal static class HotkeyThreadCall
{
    /// <summary>
    /// Espera <paramref name="task"/> e devolve a exceção original (o <c>Task.Wait</c> embrulharia numa
    /// <see cref="AggregateException"/>). Sem resposta no prazo → <c>hotkey_in_use</c>.
    /// </summary>
    public static void Wait(Task task, TimeSpan timeout)
    {
        try
        {
            task.WaitAsync(timeout).GetAwaiter().GetResult();
        }
        catch (TimeoutException) when (!task.IsCompleted)
        {
            throw new GlobalHotkeyException(GlobalHotkeyException.InUse, "A thread dos atalhos não respondeu.");
        }
    }
}

public static class GlobalHotkeys
{
    /// <summary>A implementação do sistema atual.</summary>
    public static IGlobalHotkeys Create(ILoggerFactory logs)
    {
        if (OperatingSystem.IsWindows())
            return new WindowsHotkeys(logs.CreateLogger<WindowsHotkeys>());
        if (OperatingSystem.IsLinux())
            return new X11Hotkeys(logs.CreateLogger<X11Hotkeys>());
        // macOS: fica para depois (D35); o atalho da janela continua.
        if (OperatingSystem.IsMacOS())
            return new NoHotkeys("macos");
        return new NoHotkeys("unsupported_os");
    }
}
