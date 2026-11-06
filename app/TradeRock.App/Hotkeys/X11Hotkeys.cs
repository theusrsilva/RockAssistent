using System.Collections.Concurrent;
using System.Runtime.InteropServices;

namespace TradeRock.App.Hotkeys;

/// <summary>
/// Atalho global no Linux com X11: <c>XGrabKey</c> na janela raiz, numa conexão própria com o servidor X e numa thread
/// própria (só ela fala com essa conexão). A tecla é pega com e sem NumLock/CapsLock. Se outro programa já pegou a
/// tecla, o X responde <c>BadAccess</c> (capturado por um <c>XSetErrorHandler</c> só durante o grab) → <c>hotkey_in_use</c>.
/// No Wayland o sistema não deixa (o XWayland só vê teclas com uma janela X na frente): <c>Supported=false</c>.
/// </summary>
public sealed class X11Hotkeys : IGlobalHotkeys
{
    private const string LibX11 = "libX11.so.6";
    private const int KeyPress = 2;
    private const int KeyRelease = 3;
    private const uint LockMask = 0x2; // CapsLock
    private const uint Mod2Mask = 0x10; // NumLock na maioria dos teclados
    private const uint ModifierMask = 0x1 | 0x4 | 0x8 | 0x40; // Shift, Control, Mod1 (Alt), Mod4 (Super)
    private const int GrabModeAsync = 1;
    private const byte BadAccess = 10;
    private static readonly uint[] IgnoredMasks = [0, LockMask, Mod2Mask, LockMask | Mod2Mask];

    private readonly ILogger _logger;
    private readonly object _gate = new();
    private readonly ConcurrentQueue<Action> _commands = new();
    private readonly AutoResetEvent _wake = new(false);
    private readonly Dictionary<string, (int Keycode, uint Mask)> _grabs = new(StringComparer.Ordinal);
    private readonly HashSet<int> _down = [];
    private string? _reason;
    private Thread? _thread;
    private volatile bool _stop;
    private bool _disposed;
    private IntPtr _display;
    private nuint _root;

    public X11Hotkeys(ILogger<X11Hotkeys> logger, Func<string, string?>? environment = null, Func<bool>? libraryLoads = null)
    {
        _logger = logger;
        _reason = Detect(environment ?? Environment.GetEnvironmentVariable, libraryLoads ?? LibraryLoads);
    }

    /// <summary>
    /// Motivo para não ter atalho global, ou nulo. Sessão Wayland (ou sem DISPLAY e com WAYLAND_DISPLAY) → <c>wayland</c>;
    /// sem DISPLAY ou sem a libX11 → <c>no_x11</c>.
    /// </summary>
    public static string? Detect(Func<string, string?> environment, Func<bool> libraryLoads)
    {
        if (string.Equals(environment("XDG_SESSION_TYPE"), "wayland", StringComparison.OrdinalIgnoreCase))
            return "wayland";
        if (string.IsNullOrEmpty(environment("DISPLAY")))
            return string.IsNullOrEmpty(environment("WAYLAND_DISPLAY")) ? "no_x11" : "wayland";
        return libraryLoads() ? null : "no_x11";
    }

    private static bool LibraryLoads()
    {
        if (!NativeLibrary.TryLoad(LibX11, out var handle))
            return false;
        NativeLibrary.Free(handle);
        return true;
    }

    public bool Supported
    {
        get { lock (_gate) return _reason is null; }
    }

    public string? Reason
    {
        get { lock (_gate) return _reason; }
    }

    public event Action<string>? Pressed;

    public void Register(string id, string combo)
    {
        var parsed = HotkeyCombo.Parse(combo);
        if (!HotkeyKeys.TryX11Keysym(parsed.Code, out var keysym))
            throw HotkeyKeys.Unmapped(parsed.Code);
        Run(() =>
        {
            UngrabOnThread(id);
            var keycode = XKeysymToKeycode(_display, keysym);
            if (keycode == 0)
                throw new GlobalHotkeyException(GlobalHotkeyException.Unsupported, $"O teclado não tem a tecla {parsed.Code}.");
            var mask = parsed.X11Mask;
            if (!Grab(keycode, mask))
            {
                Ungrab(keycode, mask);
                throw new GlobalHotkeyException(GlobalHotkeyException.InUse, "Outro programa já usa essa tecla.");
            }
            _grabs[id] = (keycode, mask);
        });
    }

    public void Unregister(string id)
    {
        lock (_gate)
        {
            if (_thread is null || _disposed)
                return;
        }
        Run(() => UngrabOnThread(id));
    }

    private void UngrabOnThread(string id)
    {
        if (_grabs.Remove(id, out var grab))
            Ungrab(grab.Keycode, grab.Mask);
    }

    /// <summary>Pega a tecla (com as variações de NumLock/CapsLock). Falso se o X respondeu BadAccess.</summary>
    private bool Grab(int keycode, uint mask)
    {
        lock (ErrorLock)
        {
            s_display = _display;
            s_lastError = 0;
            var previous = XSetErrorHandler(s_handlerPointer);
            try
            {
                foreach (var extra in IgnoredMasks)
                    XGrabKey(_display, keycode, mask | extra, _root, 0, GrabModeAsync, GrabModeAsync);
                XSync(_display, 0); // os erros dos grabs chegam aqui
            }
            finally
            {
                XSetErrorHandler(previous);
                s_display = IntPtr.Zero;
            }
            return s_lastError != BadAccess;
        }
    }

    private void Ungrab(int keycode, uint mask)
    {
        lock (ErrorLock)
        {
            s_display = _display;
            var previous = XSetErrorHandler(s_handlerPointer);
            try
            {
                foreach (var extra in IgnoredMasks)
                    XUngrabKey(_display, keycode, mask | extra, _root);
                XSync(_display, 0);
            }
            finally
            {
                XSetErrorHandler(previous);
                s_display = IntPtr.Zero;
            }
        }
    }

    /// <summary>Roda <paramref name="action"/> na thread do X e espera (as exceções voltam para quem chamou).</summary>
    private void Run(Action action)
    {
        var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_reason is not null)
                throw new GlobalHotkeyException(GlobalHotkeyException.GlobalUnsupported, $"Atalho global indisponível: {_reason}");
            EnsureThread();
            if (_reason is not null)
                throw new GlobalHotkeyException(GlobalHotkeyException.GlobalUnsupported, $"Atalho global indisponível: {_reason}");
        }
        _commands.Enqueue(() =>
        {
            try
            {
                action();
                done.SetResult();
            }
            catch (Exception ex)
            {
                done.SetException(ex);
            }
        });
        _wake.Set();
        if (!done.Task.Wait(TimeSpan.FromSeconds(5)))
            throw new GlobalHotkeyException(GlobalHotkeyException.InUse, "A thread dos atalhos não respondeu.");
        done.Task.GetAwaiter().GetResult();
    }

    /// <summary>Abre a conexão com o X na thread dos atalhos. Sem conexão, <see cref="Reason"/> vira <c>no_x11</c>.</summary>
    private void EnsureThread()
    {
        if (_thread is not null)
            return;
        using var ready = new ManualResetEventSlim();
        var opened = false;
        var thread = new Thread(() => Loop(ready, ok => opened = ok)) { IsBackground = true, Name = "Atalho global (X11)" };
        thread.Start();
        ready.Wait();
        if (!opened)
        {
            _reason = "no_x11";
            thread.Join();
            return;
        }
        _thread = thread;
    }

    private void Loop(ManualResetEventSlim ready, Action<bool> opened)
    {
        try
        {
            _display = XOpenDisplay(IntPtr.Zero);
        }
        catch (Exception ex) when (ex is DllNotFoundException or EntryPointNotFoundException)
        {
            _display = IntPtr.Zero;
        }
        if (_display == IntPtr.Zero)
        {
            _logger.LogWarning("Não foi possível abrir a conexão com o X11.");
            opened(false);
            ready.Set();
            return;
        }
        _root = XDefaultRootWindow(_display);
        // Segurar a tecla não manda KeyRelease no meio: um aperto conta uma vez.
        XkbSetDetectableAutoRepeat(_display, 1, out _);
        opened(true);
        ready.Set();

        var buffer = Marshal.AllocHGlobal(256); // XEvent: 24 longs
        try
        {
            while (!_stop)
            {
                while (_commands.TryDequeue(out var command))
                    command();
                while (!_stop && XPending(_display) > 0)
                {
                    XNextEvent(_display, buffer);
                    OnEvent(buffer);
                }
                _wake.WaitOne(20);
            }
        }
        finally
        {
            foreach (var grab in _grabs.Values)
                Ungrab(grab.Keycode, grab.Mask);
            _grabs.Clear();
            while (_commands.TryDequeue(out var command))
                command();
            Marshal.FreeHGlobal(buffer);
            XCloseDisplay(_display);
            _display = IntPtr.Zero;
        }
    }

    private void OnEvent(IntPtr ev)
    {
        // XKeyEvent: type, serial, send_event, display, window, root, subwindow, time (8 campos do tamanho de um
        // ponteiro), x, y, x_root, y_root (4 ints), state, keycode.
        var type = Marshal.ReadInt32(ev, 0);
        if (type is not (KeyPress or KeyRelease))
            return;
        var stateOffset = IntPtr.Size * 8 + 16;
        var state = (uint)Marshal.ReadInt32(ev, stateOffset);
        var keycode = Marshal.ReadInt32(ev, stateOffset + 4);
        if (type == KeyRelease)
        {
            _down.Remove(keycode);
            return;
        }
        if (!_down.Add(keycode))
            return; // repetição de tecla segurada
        var mask = state & ModifierMask;
        foreach (var (id, grab) in _grabs)
        {
            if (grab.Keycode != keycode || grab.Mask != mask)
                continue;
            try
            {
                Pressed?.Invoke(id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao tratar o atalho global.");
            }
            return;
        }
    }

    public void Dispose()
    {
        Thread? thread;
        lock (_gate)
        {
            if (_disposed)
                return;
            _disposed = true;
            thread = _thread;
        }
        _stop = true;
        _wake.Set();
        thread?.Join(TimeSpan.FromSeconds(2));
        _wake.Dispose();
    }

    // ---- Erros do X (o handler é global no Xlib: só fica instalado durante o grab e repassa os de outras conexões) ----

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    private delegate int XErrorHandler(IntPtr display, IntPtr errorEvent);

    private static readonly object ErrorLock = new();
    private static readonly XErrorHandler s_handler = OnXError;
    private static readonly IntPtr s_handlerPointer = Marshal.GetFunctionPointerForDelegate(s_handler);
    private static IntPtr s_display;
    private static IntPtr s_previous;
    private static byte s_lastError;

    private static int OnXError(IntPtr display, IntPtr errorEvent)
    {
        if (display == s_display)
        {
            // XErrorEvent: type, display, resourceid, serial (4 campos do tamanho de um ponteiro), error_code.
            s_lastError = Marshal.ReadByte(errorEvent, IntPtr.Size * 4);
            return 0;
        }
        return s_previous != IntPtr.Zero ? Marshal.GetDelegateForFunctionPointer<XErrorHandler>(s_previous)(display, errorEvent) : 0;
    }

    private static IntPtr XSetErrorHandler(IntPtr handler)
    {
        var previous = XSetErrorHandlerNative(handler);
        s_previous = handler == s_handlerPointer ? previous : IntPtr.Zero;
        return previous;
    }

    [DllImport(LibX11)]
    private static extern IntPtr XOpenDisplay(IntPtr displayName);

    [DllImport(LibX11)]
    private static extern int XCloseDisplay(IntPtr display);

    [DllImport(LibX11)]
    private static extern nuint XDefaultRootWindow(IntPtr display);

    [DllImport(LibX11)]
    private static extern byte XKeysymToKeycode(IntPtr display, nuint keysym);

    [DllImport(LibX11)]
    private static extern int XGrabKey(IntPtr display, int keycode, uint modifiers, nuint grabWindow, int ownerEvents,
        int pointerMode, int keyboardMode);

    [DllImport(LibX11)]
    private static extern int XUngrabKey(IntPtr display, int keycode, uint modifiers, nuint grabWindow);

    [DllImport(LibX11)]
    private static extern int XSync(IntPtr display, int discard);

    [DllImport(LibX11)]
    private static extern int XPending(IntPtr display);

    [DllImport(LibX11)]
    private static extern int XNextEvent(IntPtr display, IntPtr eventReturn);

    [DllImport(LibX11, EntryPoint = "XSetErrorHandler")]
    private static extern IntPtr XSetErrorHandlerNative(IntPtr handler);

    [DllImport(LibX11)]
    private static extern int XkbSetDetectableAutoRepeat(IntPtr display, int detectable, out int supported);
}
