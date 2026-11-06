using PKHeX.Core;

namespace TradeRock.Core.Builder;

/// <summary>
/// Uma evolução por troca possível para um Pokémon.
/// </summary>
/// <param name="RequiredItem">Item que ele precisa segurar, no número do próprio formato (o PK3 usa os números da
/// Gen 3, o PK2 os da Gen 2); null quando não precisa.</param>
/// <param name="TradeWith">Espécie com que precisa ser trocado (Karrablast ↔ Shelmet); null nas outras.</param>
public readonly record struct TradeEvolutionOption(ushort Species, byte Form, int? RequiredItem, ushort? TradeWith = null);

/// <summary>Código de erro da evolução por troca (evolve_not_available, evolve_wrong_target).</summary>
public sealed class TradeEvolutionException(string code) : Exception(code)
{
    public string Code { get; } = code;
}

/// <summary>
/// Evoluir por troca sem trocar de verdade (Boxes, Tarefa 18): as evoluções vêm da tabela do PKHeX
/// (<see cref="EvolutionTree.GetEvolutionTree"/> do formato do Pokémon, <c>Forward.GetForward</c>) e só entram os
/// métodos de troca (<see cref="EvolutionType.Trade"/>, <see cref="EvolutionType.TradeHeldItem"/> e
/// <see cref="EvolutionType.TradeShelmetKarrablast"/>). Segurando a Everstone, não evolui.
/// O resultado mantém o treinador, o PID, os IVs/EVs e os golpes (com os PP); muda a espécie, a forma, os
/// atributos, o nome (se não tiver apelido) e a habilidade no mesmo índice. O item exigido é consumido.
/// A tabela do PKHeX para a Gen 2 não guarda o item exigido (só "troca"): o item vem da tabela da Gen 3, convertido.
/// </summary>
public static class TradeEvolution
{
    private const ushort Karrablast = 588, Shelmet = 616;
    private const int EverstoneNational = 229;

    public static IReadOnlyList<TradeEvolutionOption> Options(PKM pk)
    {
        if (pk.Species == 0 || pk.IsEgg || HoldsEverstone(pk))
            return [];
        var tree = EvolutionTree.GetEvolutionTree(pk.Context);
        var list = new List<TradeEvolutionOption>();
        foreach (var m in tree.Forward.GetForward(pk.Species, pk.Form).Span)
        {
            var form = m.GetDestinationForm(pk.Form);
            TradeEvolutionOption? option = m.Method switch
            {
                EvolutionType.Trade => new(m.Species, form, Gen2Item(pk, m.Species)),
                EvolutionType.TradeHeldItem => new(m.Species, form, m.Argument),
                EvolutionType.TradeShelmetKarrablast => new(m.Species, form, null, pk.Species == Karrablast ? Shelmet : Karrablast),
                _ => null,
            };
            if (option is { } o && !list.Any(x => x.Species == o.Species && x.Form == o.Form))
                list.Add(o);
        }
        return list;
    }

    /// <summary>
    /// Devolve uma cópia evoluída (o original não muda). Sem a evolução na lista: <see cref="TradeEvolutionException"/>
    /// com <c>evolve_not_available</c> (nenhuma evolução por troca) ou <c>evolve_wrong_target</c> (outra espécie).
    /// Quem não segura o item exigido evolui mesmo assim (o app faz a troca no lugar do jogo); quem segura, perde o item.
    /// </summary>
    public static PKM Apply(PKM pk, ushort species, byte form)
    {
        var options = Options(pk);
        if (options.Count == 0)
            throw new TradeEvolutionException("evolve_not_available");
        if (options.FirstOrDefault(o => o.Species == species && o.Form == form) is not { Species: > 0 } option)
            throw new TradeEvolutionException("evolve_wrong_target");

        var result = pk.Clone();
        var nicknamed = result.IsNicknamed; // no PK3 é calculado pelo nome: ler antes de mudar a espécie
        var abilityIndex = result.Format >= 3 ? AbilityIndex(result.AbilityNumber) : -1;

        result.Species = species; // no PK1 isto também troca os tipos guardados
        result.Form = form;
        if (option.RequiredItem is { } item && result.Format >= 2 && result.HeldItem == item)
            result.HeldItem = 0;
        if (!nicknamed)
            result.ClearNickname();
        if (abilityIndex >= 0)
            result.RefreshAbility(abilityIndex);

        switch (result)
        {
            case PB7 pb7:
                pb7.ResetCalculatedValues(); // CP, altura e peso absolutos
                break;
            case PA8 pa8:
                pa8.ResetHeight();
                pa8.ResetWeight();
                break;
        }
        result.ResetPartyStats();
        result.RefreshChecksum();
        return result;
    }

    // AbilityNumber: 1, 2 ou 4 (habilidade 1, 2 ou oculta) → índice 0, 1 ou 2.
    private static int AbilityIndex(int abilityNumber) => abilityNumber switch
    {
        2 => 1,
        4 => 2,
        _ => 0,
    };

    private static bool HoldsEverstone(PKM pk) => pk.Format switch
    {
        1 => false, // a Gen 1 não tem item segurado (o byte é a taxa de captura)
        2 => pk.HeldItem == ItemConverter.GetItemOld2(EverstoneNational),
        3 => pk.HeldItem == ItemConverter.GetItemOld3(EverstoneNational),
        _ => pk.HeldItem == EverstoneNational,
    };

    /// <summary>Na Gen 2, o item exigido (King's Rock, Metal Coat...) pela tabela da Gen 3; null nas outras.</summary>
    private static int? Gen2Item(PKM pk, ushort target)
    {
        if (pk.Format != 2)
            return null;
        foreach (var m in EvolutionTree.GetEvolutionTree(EntityContext.Gen3).Forward.GetForward(pk.Species, 0).Span)
        {
            if (m.Species == target && m.Method == EvolutionType.TradeHeldItem)
                return ItemConverter.GetItemOld2(ItemConverter.GetItemFuture3(m.Argument));
        }
        return null;
    }

    /// <summary>Número nacional do item (para o nome), a partir do número do formato.</summary>
    public static int NationalItem(PKM pk, int item) => pk.Format switch
    {
        2 => ItemConverter.GetItemFuture2((byte)item),
        3 => ItemConverter.GetItemFuture3((ushort)item),
        _ => item,
    };
}
