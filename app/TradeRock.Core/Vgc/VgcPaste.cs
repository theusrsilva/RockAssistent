using PKHeX.Core;

namespace TradeRock.Core.Vgc;

/// <summary>Um Pokémon de um paste, como a lista de times mostra (sprite, nome e item).</summary>
public sealed record PasteMember(ushort Species, byte Form, string Name, string? Item);

/// <summary>Leitura simples de pastes do Showdown (mesmo parser do PKHeX que o construtor usa).</summary>
public static class VgcPaste
{
    /// <summary>Tamanho máximo do paste colado (20 KB).</summary>
    public const int MaxBytes = 20 * 1024;

    public static IReadOnlyList<ShowdownSet> Sets(string paste) =>
        ShowdownParsing.GetShowdownSets(paste.AsMemory(), BattleTemplateLocalization.Default).ToList();

    public static IReadOnlyList<PasteMember> Members(string paste)
    {
        var strings = GameInfo.GetStrings("en");
        return Sets(paste).Select(s => new PasteMember(
            s.Species,
            s.Form,
            s.Species < strings.specieslist.Length ? strings.specieslist[s.Species] : $"#{s.Species}",
            s.HeldItem > 0 && s.HeldItem < strings.itemlist.Length ? strings.itemlist[s.HeldItem] : null)).ToList();
    }
}
