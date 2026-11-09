using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TradeRock.Core.Setup;

/// <summary>
/// macOS (D35): só o ESP32, como no Windows. As portas são as /dev/cu.* (a "callout", que abre sem esperar o sinal
/// de linha). VID e PID vêm do <c>ioreg</c> quando ele responde; sem ele, só o nome da porta.
/// </summary>
public static partial class MacProbe
{
    public const string Os = "macos";

    public static SystemSnapshot Read()
    {
        var names = Directory.Exists("/dev") ? Directory.EnumerateFiles("/dev", "cu.*").ToList() : [];
        return new SystemSnapshot(Os, false, null, null, [], Ports(names, RunIoreg()), null, false, false);
    }

    /// <summary>Portas de verdade, com o que o ioreg disse delas (as do Bluetooth e do console de depuração saem).</summary>
    public static IReadOnlyList<SerialPortInfo> Ports(IEnumerable<string> devicePaths, string? ioreg)
    {
        var usb = ioreg is null ? [] : ParseIoreg(ioreg);
        return devicePaths
            .Where(p => !Ignored(Path.GetFileName(p)))
            .Distinct(StringComparer.Ordinal)
            .Order(StringComparer.Ordinal)
            .Select(p => usb.FirstOrDefault(u => u.Path == p) ?? new SerialPortInfo(p, null, null, null))
            .ToList();
    }

    private static bool Ignored(string name) =>
        name.Contains("Bluetooth", StringComparison.OrdinalIgnoreCase)
        || name.Contains("debug-console", StringComparison.OrdinalIgnoreCase)
        || name.Contains("wlan-debug", StringComparison.OrdinalIgnoreCase);

    [GeneratedRegex("^\\s*[|\\s]*\"(idVendor|idProduct|USB Product Name|IOCalloutDevice)\" = (.+?)\\s*$")]
    private static partial Regex Property();

    /// <summary>
    /// Saída de <c>ioreg -r -c IOUSBHostDevice -l</c>: um bloco por aparelho USB (linha "+-o" sem recuo), com as
    /// propriedades dele e, mais abaixo, a porta serial criada pelo driver (IOCalloutDevice).
    /// </summary>
    public static IReadOnlyList<SerialPortInfo> ParseIoreg(string text)
    {
        var result = new List<SerialPortInfo>();
        string? vid = null, pid = null, name = null;
        var paths = new List<string>();

        void Flush()
        {
            foreach (var path in paths)
                result.Add(new SerialPortInfo(path, vid, pid, name));
            (vid, pid, name) = (null, null, null);
            paths.Clear();
        }

        foreach (var line in text.Split('\n'))
        {
            if (line.StartsWith("+-o ", StringComparison.Ordinal))
            {
                Flush();
                continue;
            }
            var m = Property().Match(line);
            if (!m.Success)
                continue;
            var value = m.Groups[2].Value;
            switch (m.Groups[1].Value)
            {
                case "idVendor" when vid is null:
                    vid = Hex(value);
                    break;
                case "idProduct" when pid is null:
                    pid = Hex(value);
                    break;
                case "USB Product Name" when name is null:
                    name = value.Trim('"');
                    break;
                case "IOCalloutDevice":
                    paths.Add(value.Trim('"'));
                    break;
            }
        }
        Flush();
        return result;
    }

    private static string? Hex(string value) =>
        int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out var n) && n is >= 0 and <= 0xffff
            ? n.ToString("x4", CultureInfo.InvariantCulture)
            : null;

    private static string? RunIoreg()
    {
        const string tool = "/usr/sbin/ioreg";
        if (!File.Exists(tool))
            return null;
        try
        {
            var start = new ProcessStartInfo(tool) { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
            foreach (var arg in new[] { "-r", "-c", "IOUSBHostDevice", "-l" })
                start.ArgumentList.Add(arg);
            using var process = Process.Start(start);
            if (process is null)
                return null;
            var output = process.StandardOutput.ReadToEndAsync();
            if (!process.WaitForExit(5000))
            {
                try { process.Kill(); } catch (InvalidOperationException) { }
                return null;
            }
            return process.ExitCode == 0 ? output.GetAwaiter().GetResult() : null;
        }
        catch (Exception e) when (e is System.ComponentModel.Win32Exception or IOException)
        {
            return null;
        }
    }
}
