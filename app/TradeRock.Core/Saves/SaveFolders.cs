using System.Text;
using System.Text.RegularExpressions;
using TradeRock.Core.Storage;

namespace TradeRock.Core.Saves;

/// <summary>Uma pasta onde o app procura saves.</summary>
/// <param name="Path">Caminho absoluto da pasta.</param>
/// <param name="Pattern">Padrões separados por ";" relativos à pasta ("**/" = qualquer subpasta, "*" = qualquer nome).</param>
public sealed record SaveFolder(string Path, string Pattern);

/// <summary>Pastas de saves (configuração <c>saves.folders</c>). Começa vazia: a tela oferece os caminhos típicos.</summary>
public sealed class SaveFolders(SettingsStore settings)
{
    public const string SettingsKey = "saves.folders";
    public const string DefaultPattern = "**/*.sav;**/*.dsv;**/*.sa1;**/main;**/*.bin";
    public const int MaxFolders = 50;

    public Task<IReadOnlyList<SaveFolder>> GetAsync(CancellationToken ct) =>
        settings.GetAsync<IReadOnlyList<SaveFolder>>(SettingsKey, [], ct);

    /// <summary>Normaliza e grava. Lança <see cref="SaveInjectionException"/> ("folder_invalid") com caminho relativo ou vazio.</summary>
    public async Task<IReadOnlyList<SaveFolder>> SetAsync(IReadOnlyList<SaveFolder> folders, CancellationToken ct)
    {
        var normalized = Normalize(folders);
        await settings.SetAsync(SettingsKey, normalized, ct);
        return normalized;
    }

    public static IReadOnlyList<SaveFolder> Normalize(IReadOnlyList<SaveFolder>? folders)
    {
        var result = new List<SaveFolder>();
        foreach (var folder in folders ?? [])
        {
            var path = folder.Path?.Trim() ?? "";
            if (path.Length == 0 || !System.IO.Path.IsPathRooted(path))
                throw new SaveInjectionException("folder_invalid", $"Pasta inválida: \"{path}\".");
            path = System.IO.Path.TrimEndingDirectorySeparator(System.IO.Path.GetFullPath(path));
            var pattern = string.Join(';', (folder.Pattern ?? "").Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            if (pattern.Length == 0)
                pattern = DefaultPattern;
            if (result.All(f => f.Path != path))
                result.Add(new SaveFolder(path, pattern));
        }
        if (result.Count > MaxFolders)
            throw new SaveInjectionException("folder_invalid", "Pastas demais.");
        return result;
    }
}

/// <summary>
/// Casamento simples de caminhos relativos com os padrões das pastas ("**/*.sav;**/main"). Sem diferenciar
/// maiúsculas; "/" separa as pastas; "**/" = zero ou mais pastas; "*" e "?" não atravessam pastas.
/// </summary>
public sealed class SaveGlob
{
    private readonly Regex[] _patterns;

    public SaveGlob(string patterns)
    {
        _patterns = patterns.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(ToRegex).ToArray();
    }

    /// <param name="relativePath">Caminho relativo à pasta, com "/" ou "\".</param>
    public bool IsMatch(string relativePath)
    {
        var path = relativePath.Replace('\\', '/');
        return _patterns.Any(p => p.IsMatch(path));
    }

    private static Regex ToRegex(string pattern)
    {
        var p = pattern.Replace('\\', '/').TrimStart('/');
        var sb = new StringBuilder("^");
        for (var i = 0; i < p.Length; i++)
        {
            if (p[i] == '*' && i + 1 < p.Length && p[i + 1] == '*')
            {
                if (i + 2 < p.Length && p[i + 2] == '/')
                {
                    sb.Append("(?:.*/)?");
                    i += 2;
                }
                else
                {
                    sb.Append(".*");
                    i += 1;
                }
            }
            else if (p[i] == '*')
                sb.Append("[^/]*");
            else if (p[i] == '?')
                sb.Append("[^/]");
            else
                sb.Append(Regex.Escape(p[i].ToString()));
        }
        sb.Append('$');
        return new Regex(sb.ToString(), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));
    }
}
