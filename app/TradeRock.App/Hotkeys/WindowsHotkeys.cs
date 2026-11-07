using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace TradeRock.App.Hotkeys;

/// <summary>
/// Atalho global no Windows: <c>RegisterHotKey</c> sem janela (o <c>WM_HOTKEY</c> chega na fila da thread que registrou).
/// Uma thread STA própria registra, solta e roda o <c>GetMessage</c>; os pedidos de outras threads entram numa fila e
/// acordam a thread com um <c>PostThreadMessage</c>. O fim manda <c>WM_QUIT</c>.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class WindowsHotkeys(ILogger<WindowsHotkeys> logger) : IGlobalHotkeys
{
    private const uint WmHotkey = 0x0312;
    private const uint WmQuit = 0x0012;
    private const uint WmApp = 0x8000; // acorda a thread para a fila de pedidos
    private const uint ModNoRepeat = 0x4000; // segurar a tecla não repete o atalho
    private const int ErrorHotkeyAlreadyRegistered = 1409;

    private readonly object _gate = new();
    private readonly ConcurrentQueue<Action> _commands = new();
    private readonly Dictionary<string, int> _ids = new(StringComparer.Ordinal);
    private readonly Dictionary<int, string> _names = new();
    private int _nextId = 1;
    private Thread? _thread;
    private uint _threadId;
    private bool _disposed;

    public bool Supported => true;
    public string? Reason => null;
    public event Action<string>? Pressed;

    public void Register(string id, string combo)
    {
        var parsed = HotkeyCombo.Parse(combo);
        if (!HotkeyKeys.TryWindowsVk(parsed.Code, out var vk))
            throw HotkeyKeys.Unmapped(parsed.Code);
        Run(() =>
        {
            UnregisterOnThread(id);
            var number = _nextId++;
            if (!RegisterHotKey(IntPtr.Zero, number, parsed.WindowsModifiers | ModNoRepeat, vk))
            {
                var error = Marshal.GetLastPInvokeError();
                logger.LogWarning("RegisterHotKey falhou ({Error}) para {Combo}.", error, combo);
                throw new GlobalHotkeyException(GlobalHotkeyException.InUse,
                    error == ErrorHotkeyAlreadyRegistered ? "Outro programa já usa essa tecla." : $"RegisterHotKey falhou ({error}).");
            }
            _ids[id] = number;
            _names[number] = id;
        });
    }

    public void Unregister(string id)
    {
        lock (_gate)
        {
            if (_thread is null)
                return;
        }
        Run(() => UnregisterOnThread(id));
    }

    private void UnregisterOnThread(string id)
    {
        if (!_ids.Remove(id, out var number))
            return;
        _names.Remove(number);
        UnregisterHotKey(IntPtr.Zero, number);
    }

    /// <summary>Roda <paramref name="action"/> na thread dos atalhos e espera (as exceções voltam para quem chamou).</summary>
    private void Run(Action action)
    {
        var done = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        uint threadId;
        lock (_gate)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            EnsureThread();
            threadId = _threadId;
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
        PostThreadMessage(threadId, WmApp, UIntPtr.Zero, IntPtr.Zero);
        HotkeyThreadCall.Wait(done.Task, TimeSpan.FromSeconds(5));
    }

    private void EnsureThread()
    {
        if (_thread is not null)
            return;
        using var ready = new ManualResetEventSlim();
        _thread = new Thread(() => Loop(ready)) { IsBackground = true, Name = "Atalho global" };
        _thread.SetApartmentState(ApartmentState.STA);
        _thread.Start();
        ready.Wait();
    }

    private void Loop(ManualResetEventSlim ready)
    {
        _threadId = GetCurrentThreadId();
        // Cria a fila de mensagens da thread antes de alguém mandar o PostThreadMessage.
        PeekMessage(out _, IntPtr.Zero, 0, 0, 0);
        ready.Set();
        try
        {
            while (GetMessage(out var msg, IntPtr.Zero, 0, 0) > 0)
            {
                if (msg.message == WmHotkey && _names.TryGetValue((int)msg.wParam, out var name))
                    Raise(name);
                while (_commands.TryDequeue(out var command))
                    command();
            }
        }
        finally
        {
            foreach (var number in _names.Keys)
                UnregisterHotKey(IntPtr.Zero, number);
            _names.Clear();
            _ids.Clear();
            while (_commands.TryDequeue(out var command))
                command(); // quem esperava recebe a resposta (sem atalhos)
            // GetMessage com erro (-1) também chega aqui: o próximo pedido cria outra thread.
            lock (_gate)
                _thread = null;
        }
    }

    private void Raise(string name)
    {
        try
        {
            Pressed?.Invoke(name);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Erro ao tratar o atalho global.");
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
        if (thread is null)
            return;
        PostThreadMessage(_threadId, WmQuit, UIntPtr.Zero, IntPtr.Zero);
        thread.Join(TimeSpan.FromSeconds(2));
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Msg
    {
        public IntPtr hwnd;
        public uint message;
        public UIntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public int ptX;
        public int ptY;
        public uint lPrivate;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll", EntryPoint = "GetMessageW")]
    private static extern int GetMessage(out Msg lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll", EntryPoint = "PeekMessageW")]
    private static extern bool PeekMessage(out Msg lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);

    [DllImport("user32.dll", EntryPoint = "PostThreadMessageW", SetLastError = true)]
    private static extern bool PostThreadMessage(uint idThread, uint msg, UIntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();
}
