using System.ComponentModel;
using System.Diagnostics;
using TradeRock.Core.Engines.Worker;

namespace TradeRock.Core.Updates;

/// <summary>Escolhe o instalador pelo jeito que o app foi instalado (D29).</summary>
public static class UpdateInstallers
{
    /// <summary>Pasta do .deb (scripts/package-linux.sh).</summary>
    public const string DebAppDir = "/usr/lib/traderock/app/";

    public static InstallKind Detect(string baseDirectory, Func<string, string?> env)
    {
        if (OperatingSystem.IsWindows())
        {
            // O instalador põe o app em Arquivos de Programas, com o desinstalador do Inno Setup ao lado.
            var root = Path.GetFullPath(baseDirectory);
            return File.Exists(Path.Combine(root, "unins000.exe")) ? InstallKind.WindowsInstaller : InstallKind.None;
        }
        if (OperatingSystem.IsMacOS())
            return MacBundleOf(baseDirectory) is not null ? InstallKind.MacApp : InstallKind.None;
        if (OperatingSystem.IsLinux())
        {
            if (env("APPIMAGE") is { Length: > 0 } image && File.Exists(image))
                return InstallKind.AppImage;
            if (Path.GetFullPath(baseDirectory).StartsWith(DebAppDir, StringComparison.Ordinal))
                return InstallKind.Deb;
        }
        return InstallKind.None;
    }

    public static IUpdateInstaller Create(InstallKind kind, IPrivilegedShell shell) => kind switch
    {
        InstallKind.WindowsInstaller => new WindowsInstaller(),
        InstallKind.AppImage when !OperatingSystem.IsWindows() => new AppImageInstaller(Environment.GetEnvironmentVariable("APPIMAGE")!),
        InstallKind.Deb => new DebInstaller(shell),
        InstallKind.MacApp => new MacAppInstaller(MacBundleOf(AppContext.BaseDirectory)!, MacAppInstaller.RunAsync, MacAppInstaller.RelaunchAfterExit),
        _ => new NoInstaller(),
    };

    /// <summary>
    /// O RockAssistent.app que contém o executável (<c>…/X.app/Contents/MacOS</c>). Fora de um .app, rodando de
    /// dentro do .dmg (/Volumes) ou "transladado" pelo Gatekeeper (cópia só de leitura, quando o app não foi
    /// arrastado para Aplicativos), devolve null: aí a atualização é manual.
    /// </summary>
    public static string? MacBundleOf(string baseDirectory)
    {
        var dir = Path.TrimEndingDirectorySeparator(Path.GetFullPath(baseDirectory));
        const string tail = "/Contents/MacOS";
        if (!dir.EndsWith(tail, StringComparison.Ordinal))
            return null;
        var bundle = dir[..^tail.Length];
        if (!bundle.EndsWith(".app", StringComparison.Ordinal) || bundle.StartsWith("/Volumes/", StringComparison.Ordinal)
            || bundle.Contains("/AppTranslocation/", StringComparison.Ordinal))
            return null;
        return bundle;
    }

    /// <summary>Abre o app novo depois que este fechar (o banco e a pasta de dados são os mesmos).</summary>
    internal static void RelaunchLater(string program)
    {
        var start = new ProcessStartInfo("/bin/sh") { UseShellExecute = false };
        foreach (var arg in new[] { "-c", "sleep 3; exec \"$0\" >/dev/null 2>&1", program })
            start.ArgumentList.Add(arg);
        Process.Start(start);
    }
}

internal sealed class NoInstaller : IUpdateInstaller
{
    public InstallKind Kind => InstallKind.None;

    public Task InstallAsync(string file, string sha256, CancellationToken ct) =>
        throw new UpdateException("update_manual", "Esta instalação não se atualiza sozinha.");
}

/// <summary>
/// Windows: roda o instalador do Release em modo silencioso (o Windows pede o administrador, como na instalação) e o
/// app fecha. O instalador reabre o app como o usuário (/TRADEROCKRESTART=1, packaging/windows/traderock.iss).
/// </summary>
internal sealed class WindowsInstaller : IUpdateInstaller
{
    public InstallKind Kind => InstallKind.WindowsInstaller;

    public Task InstallAsync(string file, string sha256, CancellationToken ct)
    {
        try
        {
            Process.Start(new ProcessStartInfo(file, "/SILENT /SUPPRESSMSGBOXES /NORESTART /CLOSEAPPLICATIONS /TRADEROCKRESTART=1")
            {
                UseShellExecute = true,
                Verb = "runas",
            });
        }
        catch (Win32Exception e) when (e.NativeErrorCode == 1223)
        {
            throw new UpdateException("update_cancelled", "A permissão de administrador foi recusada.");
        }
        return Task.CompletedTask;
    }
}

/// <summary>AppImage: troca o arquivo no lugar (rename na mesma pasta) e reabre. Sem root.</summary>
[System.Runtime.Versioning.UnsupportedOSPlatform("windows")]
internal sealed class AppImageInstaller(string target) : IUpdateInstaller
{
    public InstallKind Kind => InstallKind.AppImage;

    public Task InstallAsync(string file, string sha256, CancellationToken ct)
    {
        var temp = Path.Combine(Path.GetDirectoryName(target)!, $".{Path.GetFileName(target)}.new");
        try
        {
            File.Copy(file, temp, overwrite: true);
            File.SetUnixFileMode(temp, File.GetUnixFileMode(target));
            // O app aberto continua com o arquivo antigo (a montagem usa o inode dele).
            File.Move(temp, target, overwrite: true);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
            try { File.Delete(temp); } catch (IOException) { }
            throw new UpdateException("update_no_permission", $"Sem permissão para trocar {target}: {e.Message}");
        }
        UpdateInstallers.RelaunchLater(target);
        return Task.CompletedTask;
    }
}

/// <summary>
/// .deb: o dpkg roda como root pelo pkexec (a senha é pedida pelo sistema). O pacote vai pela entrada padrão para um
/// arquivo do root e é conferido de novo, como root, antes de instalar: o arquivo do usuário nunca é usado pelo root.
/// </summary>
internal sealed class DebInstaller(IPrivilegedShell shell) : IUpdateInstaller
{
    public const string Script =
        "t=$(mktemp /tmp/traderock-update.XXXXXX) || exit 1; cat > \"$t\"; " +
        "if echo \"$1  $t\" | sha256sum -c --quiet; then dpkg -i \"$t\"; r=$?; else r=3; fi; rm -f \"$t\"; exit $r";

    public InstallKind Kind => InstallKind.Deb;

    public async Task InstallAsync(string file, string sha256, CancellationToken ct)
    {
        var (code, error) = await shell.RunAsync(Script, [sha256], async stdin =>
        {
            await using var source = File.OpenRead(file);
            await source.CopyToAsync(stdin, ct);
        }, ct);
        if (code is 126 or 127)
            throw new UpdateException("update_cancelled", "A senha do sistema não foi confirmada.");
        if (code != 0)
            throw new UpdateException("update_failed", $"dpkg terminou com o código {code}: {error}");
        UpdateInstallers.RelaunchLater("/usr/bin/traderock");
    }
}

/// <summary>
/// macOS (D35): monta o .dmg só para leitura, copia o RockAssistent.app dele (ditto) para a pasta do app atual, tira a
/// quarentena, troca as pastas e abre o app novo quando este fechar. Sem senha: a pasta Aplicativos aceita escrita
/// dos administradores do Mac; sem permissão, a atualização é manual.
/// </summary>
internal sealed class MacAppInstaller(
    string bundle,
    Func<string, IReadOnlyList<string>, Task<(int Code, string Error)>> run,
    Action<string> relaunch) : IUpdateInstaller
{
    public const string AppName = "RockAssistent.app";

    public InstallKind Kind => InstallKind.MacApp;

    public async Task InstallAsync(string file, string sha256, CancellationToken ct)
    {
        var parent = Path.GetDirectoryName(bundle)!;
        var name = Path.GetFileName(bundle);
        var staging = Path.Combine(parent, $".{name}.new");
        var old = Path.Combine(parent, $".{name}.old");
        var mount = Path.Combine(Path.GetTempPath(), "traderock-update-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(mount);
        try
        {
            var (code, error) = await run("/usr/bin/hdiutil", ["attach", "-nobrowse", "-readonly", "-noautoopen", "-mountpoint", mount, file]);
            if (code != 0)
                throw new UpdateException("update_failed", $"hdiutil attach terminou com o código {code}: {error}");
            try
            {
                var source = Path.Combine(mount, AppName);
                if (!Directory.Exists(source))
                    throw new UpdateException("update_failed", $"O .dmg não tem o {AppName}.");
                DeleteQuietly(staging);
                (code, error) = await run("/usr/bin/ditto", [source, staging]);
                if (code != 0)
                {
                    DeleteQuietly(staging);
                    throw new UpdateException("update_no_permission", $"Sem permissão para escrever em {parent}: {error}");
                }
            }
            finally
            {
                if ((await run("/usr/bin/hdiutil", ["detach", mount])).Code != 0)
                    await run("/usr/bin/hdiutil", ["detach", "-force", mount]);
            }

            // Baixado pelo próprio app, o arquivo não costuma vir com a quarentena; se vier, o Gatekeeper bloquearia.
            await run("/usr/bin/xattr", ["-dr", "com.apple.quarantine", staging]);

            try
            {
                DeleteQuietly(old);
                Directory.Move(bundle, old);
                Directory.Move(staging, bundle);
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                if (!Directory.Exists(bundle) && Directory.Exists(old))
                    Directory.Move(old, bundle);
                DeleteQuietly(staging);
                throw new UpdateException("update_no_permission", $"Sem permissão para trocar {bundle}: {e.Message}");
            }
            // O app aberto continua com os arquivos que já abriu.
            DeleteQuietly(old);
        }
        finally
        {
            try { Directory.Delete(mount); } catch (IOException) { }
        }
        relaunch(bundle);
    }

    private static void DeleteQuietly(string dir)
    {
        try
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, recursive: true);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException)
        {
        }
    }

    public static async Task<(int Code, string Error)> RunAsync(string program, IReadOnlyList<string> args)
    {
        var start = new ProcessStartInfo(program) { UseShellExecute = false, RedirectStandardOutput = true, RedirectStandardError = true };
        foreach (var arg in args)
            start.ArgumentList.Add(arg);
        using var process = Process.Start(start) ?? throw new UpdateException("update_failed", $"Não deu para rodar {program}.");
        var output = process.StandardOutput.ReadToEndAsync();
        var error = await process.StandardError.ReadToEndAsync();
        await output;
        await process.WaitForExitAsync();
        return (process.ExitCode, error.Trim());
    }

    /// <summary>Espera este processo terminar e abre o .app pelo "open" (como um clique no Finder).</summary>
    public static void RelaunchAfterExit(string bundle)
    {
        var start = new ProcessStartInfo("/bin/sh") { UseShellExecute = false };
        foreach (var arg in new[] { "-c", "while kill -0 \"$1\" 2>/dev/null; do sleep 1; done; exec /usr/bin/open \"$0\" >/dev/null 2>&1",
                     bundle, Environment.ProcessId.ToString(System.Globalization.CultureInfo.InvariantCulture) })
            start.ArgumentList.Add(arg);
        Process.Start(start);
    }
}
