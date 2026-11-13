using System.Drawing;

namespace TradeRock.App;

/// <summary>Preferências da mini janela do Contador Hunt (D40): virar ao minimizar e a última posição.</summary>
public sealed record MiniWindowSettings(bool OnMinimize = true, int? Left = null, int? Top = null)
{
    public const string Key = "mini_window";
}

/// <summary>Contas da mini janela, sem a janela nativa (para testar).</summary>
public static class MiniWindow
{
    /// <summary>Tamanho da mini janela a 100% de escala.</summary>
    public static readonly Size BaseSize = new(340, 230);

    public const int Margin = 16;

    /// <summary>Tamanho em pixels da tela: o Windows conta em pixels físicos (DPI 96 = 100%).</summary>
    public static Size SizeFor(uint dpi, bool windows)
    {
        var scale = windows && dpi > 96 ? dpi / 96.0 : 1.0;
        return new Size((int)Math.Round(BaseSize.Width * scale), (int)Math.Round(BaseSize.Height * scale));
    }

    /// <summary>
    /// Onde a mini janela abre: a última posição, se ainda cabe inteira na área de trabalho; senão o canto inferior
    /// direito, com margem.
    /// </summary>
    public static Point Place(Rectangle workArea, Size size, Point? saved)
    {
        if (saved is { } p && workArea.Contains(new Rectangle(p, size)))
            return p;
        return new Point(Math.Max(workArea.Left, workArea.Right - size.Width - Margin),
            Math.Max(workArea.Top, workArea.Bottom - size.Height - Margin));
    }
}
