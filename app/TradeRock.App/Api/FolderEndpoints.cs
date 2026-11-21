namespace TradeRock.App.Api;

/// <summary>
/// Navegador de pastas do PC para quem está no celular (modo servidor, só administradores: <c>AccessPolicy</c>). Substitui o
/// diálogo nativo de escolher pasta, que abriria na tela do PC. Lista só pastas (nunca arquivos nem conteúdo); quem chama
/// acrescenta a escolhida pelo <c>PUT /api/settings/save-folders</c>.
/// </summary>
public static class FolderEndpoints
{
    /// <summary>Limite de pastas devolvidas de uma vez (uma pasta com milhares de subpastas não trava o celular).</summary>
    public const int MaxEntries = 500;

    public sealed record FolderEntryDto(string Name, string Path, bool Hidden);

    public sealed record FolderRootDto(string Label, string Path);

    /// <param name="Parent">A pasta de cima (null na raiz).</param>
    /// <param name="Truncated">Havia mais que <see cref="MaxEntries"/> pastas.</param>
    public sealed record FolderListDto(string Path, string? Parent, IReadOnlyList<FolderEntryDto> Folders, IReadOnlyList<FolderRootDto> Roots,
        bool Truncated);

    public static void MapFolders(this IEndpointRouteBuilder api)
    {
        // Sem "path": a pasta pessoal. Erros: folder_invalid (caminho relativo), folder_missing (404), folder_denied (403).
        api.MapGet("/fs/folders", (string? path) =>
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var target = string.IsNullOrWhiteSpace(path) ? home : path.Trim();
            if (!Path.IsPathFullyQualified(target))
                return Results.BadRequest(new { error = "folder_invalid" });
            target = Path.GetFullPath(target);
            if (!Directory.Exists(target))
                return Results.NotFound(new { error = "folder_missing" });
            try
            {
                return Results.Ok(List(target, home));
            }
            catch (Exception e) when (e is UnauthorizedAccessException or IOException)
            {
                return Results.Json(new { error = "folder_denied" }, statusCode: StatusCodes.Status403Forbidden);
            }
        });
    }

    public static FolderListDto List(string folder, string home)
    {
        var all = new DirectoryInfo(folder)
            .EnumerateDirectories("*", new EnumerationOptions { IgnoreInaccessible = true, AttributesToSkip = FileAttributes.System })
            .Select(d => new FolderEntryDto(d.Name, d.FullName, d.Name.StartsWith('.') || d.Attributes.HasFlag(FileAttributes.Hidden)))
            .Take(MaxEntries + 1)
            .ToList();
        var folders = all.Take(MaxEntries)
            .OrderBy(f => f.Hidden)
            .ThenBy(f => f.Name, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
        var parent = Directory.GetParent(folder)?.FullName;
        return new FolderListDto(folder, parent, folders, Roots(home), all.Count > MaxEntries);
    }

    /// <summary>Atalhos: a pasta pessoal e, no Windows, as unidades; no Linux e no Mac, a raiz.</summary>
    private static List<FolderRootDto> Roots(string home)
    {
        var roots = new List<FolderRootDto> { new("~", home) };
        if (OperatingSystem.IsWindows())
        {
            roots.AddRange(DriveInfo.GetDrives()
                .Where(d => d.IsReady && d.DriveType is DriveType.Fixed or DriveType.Removable or DriveType.Network)
                .Select(d => new FolderRootDto(d.Name.TrimEnd('\\'), d.RootDirectory.FullName)));
        }
        else
        {
            roots.Add(new FolderRootDto("/", "/"));
        }
        return roots;
    }
}
