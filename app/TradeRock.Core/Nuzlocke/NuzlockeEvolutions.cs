using PKHeX.Core;
using TradeRock.Core.Builder;
using TradeRock.Core.Dex;

namespace TradeRock.Core.Nuzlocke;

/// <summary>
/// Evoluções válidas de um Pokémon da run pela tabela do PKHeX do jogo (<see cref="EvolutionTree.GetEvolutionTree"/>,
/// <c>Forward.GetForward(espécie, forma)</c>, forma de destino por <c>GetDestinationForm</c>): o Meowth de Galar só vira
/// Perrserker, o de Kanto só Persian, o Rattata de Alola vira o Raticate de Alola.
/// <para>A forma do Nuzlocke é a posição na lista de formas do PokeAPI (0 = a padrão; <see cref="DexService.PokemonForms"/>).
/// Para o PKHeX ela vira o índice de forma pelo sufixo do identificador (<c>meowth-galar</c> → a forma "Galarian" da
/// lista de formas do PKHeX em inglês), e o destino volta pelo mesmo caminho. Sufixo sem par no PKHeX = forma 0.</para>
/// </summary>
public static class NuzlockeEvolutions
{
    /// <summary>
    /// (espécie, forma do Nuzlocke) de cada evolução válida; null quando o jogo não tem tabela no PKHeX (aí vale a cadeia
    /// do PokeAPI como está).
    /// </summary>
    public static IReadOnlyList<(int Species, int Form)>? Next(DexService dex, string gameKey, int species, int form)
    {
        if (!PkhexAvailability.Games.TryGetValue(gameKey, out var game) || species is < 1 or > ushort.MaxValue)
            return null;
        var context = game.Context;
        var tree = EvolutionTree.GetEvolutionTree(context);
        var pkForm = ToPkhexForm(dex, context, species, form);
        var result = new List<(int, int)>();
        foreach (var m in tree.Forward.GetForward((ushort)species, pkForm).Span)
        {
            if (m.Species == 0)
                continue;
            var next = (m.Species, FromPkhexForm(dex, context, m.Species, m.GetDestinationForm(pkForm)));
            if (!result.Contains(next))
                result.Add(next);
        }
        return result;
    }

    private static byte ToPkhexForm(DexService dex, EntityContext context, int species, int form)
    {
        if (form <= 0)
            return 0;
        var suffix = Suffix(dex, species, form);
        if (suffix is null)
            return 0;
        var names = FormNames(species, context);
        for (var i = 1; i < names.Length; i++)
            if (SameForm(names[i], suffix))
                return (byte)i;
        return 0;
    }

    private static int FromPkhexForm(DexService dex, EntityContext context, int species, byte pkForm)
    {
        if (pkForm == 0)
            return 0;
        var names = FormNames(species, context);
        if (pkForm >= names.Length)
            return 0;
        var forms = dex.PokemonForms(species);
        var baseId = forms.Count > 0 ? forms[0] : null;
        for (var i = 1; i < forms.Count; i++)
            if (baseId is not null && forms[i].StartsWith(baseId + "-", StringComparison.Ordinal) && SameForm(names[pkForm], forms[i][(baseId.Length + 1)..]))
                return i;
        return 0;
    }

    private static string[] FormNames(int species, EntityContext context) =>
        FormConverter.GetFormList((ushort)species, BuilderData.Strings.types, BuilderData.Strings.forms, context);

    /// <summary>Sufixo do identificador da forma (<c>meowth-galar</c> → <c>galar</c>).</summary>
    private static string? Suffix(DexService dex, int species, int form)
    {
        var forms = dex.PokemonForms(species);
        if (form >= forms.Count || forms.Count == 0)
            return null;
        var baseId = forms[0];
        return forms[form].StartsWith(baseId + "-", StringComparison.Ordinal) ? forms[form][(baseId.Length + 1)..] : null;
    }

    /// <summary>"Galarian" e <c>galar</c>, "Alolan" e <c>alola</c>: uma começa com a outra (só letras, minúsculas).</summary>
    private static bool SameForm(string pkhexName, string suffix)
    {
        var a = new string(pkhexName.Where(char.IsLetter).Select(char.ToLowerInvariant).ToArray());
        var b = new string(suffix.Where(char.IsLetter).Select(char.ToLowerInvariant).ToArray());
        return a.Length > 0 && b.Length > 0 && (a.StartsWith(b, StringComparison.Ordinal) || b.StartsWith(a, StringComparison.Ordinal));
    }
}
