namespace TradeRock.Core.GameData;

/// <summary>
/// Caixa aproximada do rótulo de um lugar, em unidades da grade. Espelha <c>labelLayout</c> de
/// <c>frontend/src/pages/nuzlocke/regionMap.ts</c> (mesmas medidas): mude os dois juntos.
/// </summary>
public static class MapLabels
{
    /// <summary>Unidades do desenho por unidade da grade.</summary>
    public const double Cell = 32;
    public const double CharWidth = 0.6;
    public const double LineHeight = 1.2;
    public const double Gap = 4;
    public const int WrapAfter = 12;

    public static readonly IReadOnlySet<string> Positions = new HashSet<string> { "n", "s", "e", "w", "ne", "nw", "se", "sw" };

    /// <summary>Meia largura do desenho do lugar (quadrado da cidade, quadrado pequeno da construção, círculo).</summary>
    public static double HalfSize(string kind) => kind switch { "city" => 11, "building" => 7.5, _ => 8 };

    public static double FontSize(string kind) => kind == "city" ? 13 : 11.5;

    /// <summary>Texto do rótulo: cidade sem " Town"/" City"; acima de 12 letras quebra em duas linhas no espaço do meio.</summary>
    public static IReadOnlyList<string> Lines(string kind, string name)
    {
        var text = kind == "city" ? StripSuffix(name) : name;
        if (text.Length <= WrapAfter || !text.Contains(' '))
            return [text];
        var mid = text.Length / 2.0;
        var best = -1;
        for (var i = 0; i < text.Length; i++)
            if (text[i] == ' ' && (best < 0 || Math.Abs(i - mid) < Math.Abs(best - mid)))
                best = i;
        return [text[..best], text[(best + 1)..]];
    }

    private static string StripSuffix(string name)
    {
        foreach (var suffix in new[] { " Town", " City" })
            if (name.EndsWith(suffix, StringComparison.Ordinal))
                return name[..^suffix.Length];
        return name;
    }

    /// <summary>Caixa (x, y, largura, altura) do rótulo, em unidades da grade.</summary>
    public static (double X, double Y, double W, double H) Box(MapPlace p)
    {
        var lines = Lines(p.Kind, p.Name);
        var font = FontSize(p.Kind);
        var w = lines.Max(l => l.Length) * font * CharWidth;
        var h = lines.Count * font * LineHeight;
        var half = HalfSize(p.Kind);
        double cx = p.X * Cell, cy = p.Y * Cell;
        var d = half + Gap;
        var (x, y) = p.Label switch
        {
            "n" => (cx - w / 2, cy - d - h),
            "s" => (cx - w / 2, cy + d),
            "e" => (cx + d, cy - h / 2),
            "w" => (cx - d - w, cy - h / 2),
            "ne" => (cx + half, cy - half - h),
            "nw" => (cx - half - w, cy - half - h),
            "se" => (cx + half, cy + half),
            _ => (cx - half - w, cy + half),
        };
        if (p.LabelOffset is [var dx, var dy])
        {
            x += dx * Cell;
            y += dy * Cell;
        }
        return (x / Cell, y / Cell, w / Cell, h / Cell);
    }
}
