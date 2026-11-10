using PKHeX.Core;
using TradeRock.Core.Games;

namespace TradeRock.Core.Pokemon;

/// <summary>
/// Saída do Sword/Shield a partir do original guardado (D37). Quando um Pokémon do BDSP, do Arceus ou do
/// Scarlet/Violet vai para o Sword/Shield, a conversão perde a origem (<see cref="HomeTransfer.OriginLostInSwsh"/>) e
/// o cofre guarda o arquivo de antes da ida. Na saída, a conversão parte desse original (com o encontro de verdade) e
/// as mudanças feitas no Sword/Shield são reaplicadas por cima, uma a uma, só enquanto o resultado continuar legal ou
/// "não verificável" (o mesmo critério do <see cref="HomeTransfer.Move"/>). Nenhum dado de encontro é inventado: o que
/// não puder ser levado é descartado, com uma nota <c>change_dropped_*</c>.
///
/// "Mudança" é a diferença entre o PK8 de agora e o PK8 que o original daria ao chegar (a mesma conversão, refeita):
/// assim o que a própria ida trocou (golpes que o Sword/Shield não tem, por exemplo) não conta como mudança, e o
/// original sai com o que tinha.
///
/// Reaplicado: evolução (espécie e forma), nível/experiência, golpes e PP Ups, EVs, apelido, item, menta (natureza dos
/// atributos), Hyper Training, habilidade (cápsula/patch) e as fitas do Sword/Shield (campeão de Galar, Battle Tower,
/// Master Rank). Fica de fora o que só existe no Sword/Shield (nível Dynamax, Gigantamax, TRs usados), as memórias e a
/// amizade (dependem de quem é o treinador atual) e as marcações da caixa.
/// </summary>
public static class KeptOriginal
{
    /// <summary>Nota da ida: o original ficou guardado.</summary>
    public const string KeptNote = "original_kept";

    /// <summary>Nota da saída: saiu com o encontro do original.</summary>
    public const string RestoredNote = "original_restored";

    /// <summary>Prefixo das notas de mudança que não pôde ser levada (species, level, moves, evs...).</summary>
    public const string DroppedPrefix = "change_dropped_";

    private static readonly SwitchGame Swsh = GameCatalog.Get(GameFamily.Swsh);

    /// <summary>
    /// O PK8 de agora ainda é o Pokémon do original guardado: mesma constante de encriptação, mesmo PID, mesmo
    /// treinador original e espécie igual ou evolução da do original (a edição do cofre pode ter posto outro Pokémon
    /// no registro).
    /// </summary>
    public static bool SamePokemon(PKM current, PKM original)
    {
        if (current is not PK8 || original is PK8 || current.IsEgg)
            return false;
        if (current.EncryptionConstant != original.EncryptionConstant || current.PID != original.PID)
            return false;
        if (current.TID16 != original.TID16 || current.SID16 != original.SID16
            || current.OriginalTrainerName != original.OriginalTrainerName)
            return false;
        return current.Species == original.Species || EvolvesInto(original.Species, original.Form, current.Species);
    }

    /// <summary>A espécie de agora está na linha evolutiva (para a frente) da espécie do original, pela tabela da Gen 8.</summary>
    private static bool EvolvesInto(ushort species, byte form, ushort target)
    {
        var tree = EvolutionTree.GetEvolutionTree(EntityContext.Gen8);
        var pending = new Queue<(ushort Species, byte Form, int Depth)>();
        pending.Enqueue((species, form, 0));
        while (pending.TryDequeue(out var node))
        {
            foreach (var m in tree.Forward.GetForward(node.Species, node.Form).Span)
            {
                if (m.Species == target)
                    return true;
                if (node.Depth < 2)
                    pending.Enqueue((m.Species, m.GetDestinationForm(node.Form), node.Depth + 1));
            }
        }
        return false;
    }

    /// <summary>
    /// Destinos de quem tem original guardado (código de recusa por jogo; null = pode), ensaiados com a mesma
    /// conversão de <see cref="Move"/>. Null quando o original não serve (<see cref="SamePokemon"/>).
    /// </summary>
    public static IReadOnlyDictionary<GameFamily, string?>? Destinations(PKM current, PKM original, GameFamily originalFamily)
    {
        if (!SamePokemon(current, original))
            return null;
        var result = new Dictionary<GameFamily, string?>();
        foreach (var game in GameCatalog.All)
        {
            result[game.Family] = game.Family == GameFamily.Swsh
                ? "same_game"
                : Move(current, original, originalFamily, game, new SimpleTrainerInfo(GameCatalog.ToVersion(game.Versions[0])))!.Refusal;
        }
        return result;
    }

    /// <summary>
    /// Tira do Sword/Shield a partir do original guardado. Null quando o original não serve (<see cref="SamePokemon"/>):
    /// aí vale o <see cref="HomeTransfer.Move"/> comum, que recusa quem perdeu a origem.
    /// </summary>
    /// <param name="current">O PK8 de agora.</param>
    /// <param name="original">O arquivo de antes da ida ao Sword/Shield.</param>
    /// <param name="originalFamily">O jogo do original.</param>
    public static HomeTransferResult? Move(PKM current, PKM original, GameFamily originalFamily, SwitchGame to, ITrainerInfo owner)
    {
        if (!SamePokemon(current, original))
            return null;
        if (to.Family == GameFamily.Swsh)
            return new HomeTransferResult(null, "same_game", []);
        try
        {
            return Leave(current, original, originalFamily, to, owner);
        }
        catch (Exception e) when (e is not OutOfMemoryException)
        {
            // Como no Move: nenhuma exceção do PKHeX sai daqui.
            return new HomeTransferResult(null, "no_route", [e.GetType().Name]);
        }
    }

    private static HomeTransferResult Leave(PKM current, PKM original, GameFamily originalFamily, SwitchGame to, ITrainerInfo owner)
    {
        var notes = new List<string> { RestoredNote };
        PKM result;
        if (to.Family == originalFamily)
        {
            // De volta ao próprio jogo: o original no formato nativo, sem conversão.
            result = Returned(original, owner);
        }
        else
        {
            var moved = HomeTransfer.Move(original, to, owner);
            if (!moved.Ok)
                return moved;
            result = moved.Pokemon!;
            notes.AddRange(moved.Notes);
        }

        // Como o original chegou ao Sword/Shield: a base para saber o que mudou lá.
        var arrived = HomeTransfer.Move(original, Swsh, owner).Pokemon;
        if (arrived is null)
            return new HomeTransferResult(result, null, notes);

        // Só entra mudança que não piora: se o original já era ilegal antes de ir, não há o que conferir.
        var strict = HomeTransfer.Accepted(result);
        foreach (var change in Changes)
        {
            if (!change.Differs(arrived, current))
                continue;
            var candidate = result.Clone();
            var applied = change.Apply(current, candidate, to);
            if (applied != Applied.No)
            {
                candidate.ResetPartyStats();
                candidate.RefreshChecksum();
                if (!strict || HomeTransfer.Accepted(candidate))
                    result = candidate;
                else
                    applied = Applied.No;
            }
            if (applied != Applied.All)
                notes.Add(DroppedPrefix + change.Name);
        }
        return new HomeTransferResult(result, null, notes);
    }

    /// <summary>O original de volta ao próprio jogo, com o treinador atual acertado como o jogo faria (se continuar aceito).</summary>
    private static PKM Returned(PKM original, ITrainerInfo owner)
    {
        var pk = original.Clone();
        if (pk is IHandlerUpdate handler)
            handler.UpdateHandler(owner);
        else if (pk is PB8 pb8)
            pb8.UpdateHandler(owner);
        pk.ResetPartyStats();
        pk.RefreshChecksum();
        return HomeTransfer.Accepted(pk) || !HomeTransfer.Accepted(original) ? pk : original.Clone();
    }

    private enum Applied
    {
        No,
        Partial,
        All,
    }

    /// <summary>Uma mudança que o jogador pode fazer no Sword/Shield: como notar e como passar para o resultado.</summary>
    private sealed record Change(string Name, Func<PKM, PKM, bool> Differs, Func<PKM, PKM, SwitchGame, Applied> Apply);

    // A ordem importa: a evolução e o nível vêm antes dos golpes (que dependem dos dois).
    private static readonly Change[] Changes =
    [
        new("species", (a, b) => a.Species != b.Species || a.Form != b.Form, Evolve),
        new("level", (a, b) => a.EXP != b.EXP, Level),
        new("moves", (a, b) => !a.Moves.SequenceEqual(b.Moves) || !PpUps(a).SequenceEqual(PpUps(b)), Moves),
        new("evs", (a, b) => !Evs(a).SequenceEqual(Evs(b)), (from, to, game) =>
        {
            if (to is PA8)
                return Applied.No; // o Arceus não tem EVs
            to.SetEVs(Evs(from));
            return Applied.All;
        }),
        new("nickname", (a, b) => a.IsNicknamed != b.IsNicknamed || (a.IsNicknamed && a.Nickname != b.Nickname), (from, to, game) =>
        {
            if (from.IsNicknamed)
            {
                to.Nickname = from.Nickname;
                to.IsNicknamed = true;
            }
            else
            {
                to.ClearNickname();
            }
            return Applied.All;
        }),
        new("item", (a, b) => a.HeldItem != b.HeldItem, (from, to, game) =>
        {
            if (to is PA8)
                return Applied.No; // o Arceus não tem item segurado
            to.HeldItem = from.HeldItem; // do Sword/Shield em diante os jogos usam os mesmos números de item
            return Applied.All;
        }),
        new("nature", (a, b) => a.StatAlignment != b.StatAlignment, (from, to, game) =>
        {
            to.StatAlignment = from.StatAlignment;
            return Applied.All;
        }),
        new("hyper_training", (a, b) => HyperTraining(a) != HyperTraining(b), (from, to, game) =>
        {
            if (to is not IHyperTrain target)
                return Applied.No;
            target.HyperTrainFlags = HyperTraining(from);
            return Applied.All;
        }),
        new("ability", (a, b) => a.AbilityNumber != b.AbilityNumber, (from, to, game) =>
        {
            to.RefreshAbility(AbilityIndex(from.AbilityNumber));
            return Applied.All;
        }),
        new("ribbons", (a, b) => GalarRibbons(a) != GalarRibbons(b), (from, to, game) =>
        {
            if (from is not IRibbonSetCommon8 source || to is not IRibbonSetCommon8 target)
                return Applied.No;
            target.RibbonChampionGalar |= source.RibbonChampionGalar;
            target.RibbonTowerMaster |= source.RibbonTowerMaster;
            target.RibbonMasterRank |= source.RibbonMasterRank;
            return Applied.All;
        }),
    ];

    /// <summary>Evolução feita no Sword/Shield: a espécie tem que existir no destino; a análise decide se vale.</summary>
    private static Applied Evolve(PKM from, PKM to, SwitchGame game)
    {
        if (!game.HasSpecies(from.Species, from.Form))
            return Applied.No;
        var nicknamed = to.IsNicknamed;
        var ability = AbilityIndex(to.AbilityNumber);
        to.Species = from.Species;
        to.Form = from.Form;
        if (!nicknamed)
            to.ClearNickname();
        to.RefreshAbility(ability);
        if (to is PA8 pa8)
        {
            pa8.ResetHeight();
            pa8.ResetWeight();
        }
        return Applied.All;
    }

    private static Applied Level(PKM from, PKM to, SwitchGame game)
    {
        // A linha evolutiva tem a mesma curva de experiência; se não bater, vale o nível.
        to.EXP = from.EXP;
        if (to.CurrentLevel != from.CurrentLevel)
            to.CurrentLevel = from.CurrentLevel;
        return Applied.All;
    }

    /// <summary>Os golpes de agora; se algum não valer no destino, só os que valem (os lugares vagos ficam vazios).</summary>
    private static Applied Moves(PKM from, PKM to, SwitchGame game)
    {
        var before = HomeTransfer.Accepted(to);
        SetMoves(to, from.Moves, PpUps(from));
        if (!before)
            return Applied.All;
        to.RefreshChecksum();
        var la = new LegalityAnalysis(to);
        if (HomeTransfer.Accepted(to, la))
            return Applied.All;

        var moves = from.Moves;
        var ups = PpUps(from);
        var valid = Enumerable.Range(0, 4).Where(i => moves[i] != 0 && la.Info.Moves[i].Valid).ToList();
        if (valid.Count == 0)
            return Applied.No;
        var kept = new ushort[4];
        var keptUps = new int[4];
        for (var i = 0; i < valid.Count; i++)
            (kept[i], keptUps[i]) = (moves[valid[i]], ups[valid[i]]);
        SetMoves(to, kept, keptUps);
        return Applied.Partial;
    }

    private static void SetMoves(PKM pk, ushort[] moves, int[] ppUps)
    {
        pk.SetMoves(moves);
        pk.Move1_PPUps = moves[0] == 0 ? 0 : ppUps[0];
        pk.Move2_PPUps = moves[1] == 0 ? 0 : ppUps[1];
        pk.Move3_PPUps = moves[2] == 0 ? 0 : ppUps[2];
        pk.Move4_PPUps = moves[3] == 0 ? 0 : ppUps[3];
        pk.HealPP();
        HomeTransfer.Permit(pk);
    }

    private static int[] PpUps(PKM pk) => [pk.Move1_PPUps, pk.Move2_PPUps, pk.Move3_PPUps, pk.Move4_PPUps];

    private static int[] Evs(PKM pk) => [pk.EV_HP, pk.EV_ATK, pk.EV_DEF, pk.EV_SPE, pk.EV_SPA, pk.EV_SPD];

    private static byte HyperTraining(PKM pk) => pk is IHyperTrain hyper ? hyper.HyperTrainFlags : (byte)0;

    private static int GalarRibbons(PKM pk) => pk is IRibbonSetCommon8 r
        ? (r.RibbonChampionGalar ? 1 : 0) | (r.RibbonTowerMaster ? 2 : 0) | (r.RibbonMasterRank ? 4 : 0)
        : 0;

    // AbilityNumber: 1, 2 ou 4 (habilidade 1, 2 ou oculta) → índice 0, 1 ou 2.
    private static int AbilityIndex(int abilityNumber) => abilityNumber switch
    {
        2 => 1,
        4 => 2,
        _ => 0,
    };
}
