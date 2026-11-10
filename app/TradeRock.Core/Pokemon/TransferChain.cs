using PKHeX.Core;
using TradeRock.Core.Games;

namespace TradeRock.Core.Pokemon;

/// <summary>Etapa oficial de transferência entre gerações (D34).</summary>
public enum TransferStep
{
    /// <summary>Poké Transporter a partir do Virtual Console (Red/Blue/Yellow e Gold/Silver/Crystal do 3DS) para a Gen 7.</summary>
    VirtualConsole,

    /// <summary>Pal Park (Gen 3 → Diamond/Pearl/Platinum/HeartGold/SoulSilver).</summary>
    PalPark,

    /// <summary>Poké Transfer (Gen 4 → Black/White/Black 2/White 2).</summary>
    PokeTransfer,

    /// <summary>Poké Transporter (Gen 5 → Pokémon Bank → X/Y/Omega Ruby/Alpha Sapphire).</summary>
    PokeTransporter,

    /// <summary>Pokémon Bank (Gen 6 → Sun/Moon/Ultra Sun/Ultra Moon).</summary>
    PokemonBank,

    /// <summary>Pokémon HOME (Gen 7 e Let's Go → jogos do Switch; o Let's Go não recebe).</summary>
    Home,
}

/// <summary>Um destino possível ao adaptar: o caminho e o motivo da recusa (null = pode).</summary>
public sealed record AdaptOption(StorageFormat Target, IReadOnlyList<TransferStep> Steps, string? Refusal, string? RefusedAt);

/// <summary>
/// "Adaptar para um jogo mais novo" (D34): leva um Pokémon para a frente pelos caminhos oficiais de transferência,
/// sempre para a frente, com as restrições de cada etapa do caminho aplicadas de uma vez. Conversão do PKHeX
/// (<c>PK1/PK2.ConvertToPK7</c>, <c>PK3.ConvertToPK4</c>, <c>PK4.ConvertToPK5</c>, <c>PK5.ConvertToPK6</c>,
/// <c>PK6.ConvertToPK7</c>; o HOME pelo <see cref="HomeTransfer"/>, que usa o <c>EntityConverter.ConvertToType</c>).
/// Regras (fontes na D34):
/// - Virtual Console: ovos não; Dizzy Punch (Ovo Estranho) bloqueia; Mew só o do evento do Virtual Console (OT "GF",
///   TID 22796); Celebi só a partir do nível 30 (o do evento do Crystal do VC). Na Gen 2 o item volta para a mochila do
///   jogo; na Gen 1, o "item" é a taxa de captura e se perde.
/// - Pal Park: ovos não, golpes de HM da Gen 3 não, Mail não; os outros itens seguem com o Pokémon.
/// - Poké Transfer: ovos não, HMs da Gen 4 (as dos dois pares: Cut, Fly, Surf, Strength, Rock Smash, Waterfall, Rock
///   Climb; Defog sai do HGSS e Whirlpool do DPPt) não, Pichu de orelha espetada não; o item volta para a mochila.
/// - Poké Transporter e Pokémon Bank: ovos não, Kyurem fundido não, Pikachu fantasiado (Gen 6) não; HMs passam; o
///   item volta para a mochila.
/// - HOME: ovos não, o Let's Go não recebe, a espécie e a forma precisam existir no jogo; o item volta para a mochila.
/// </summary>
public static class TransferChain
{
    private const ushort Mew = 151, Celebi = 251, Pichu = 172, Kyurem = 646, Pikachu = 25;
    private const ushort DizzyPunch = 146;

    /// <summary>HMs da Gen 3 (RSE e FRLG): Cut, Fly, Surf, Strength, Flash, Rock Smash, Waterfall e Dive.</summary>
    private static readonly HashSet<ushort> HiddenMoves3 = [15, 19, 57, 70, 148, 249, 127, 291];

    /// <summary>HMs nos dois pares da Gen 4 (Cut, Fly, Surf, Strength, Rock Smash, Waterfall, Rock Climb).</summary>
    private static readonly HashSet<ushort> HiddenMoves4 = [15, 19, 57, 70, 249, 127, 431];

    /// <summary>Mails da Gen 3 (IDs internos 121 a 132).</summary>
    private static bool IsMail3(int item) => item is >= 121 and <= 132;

    private static readonly (StorageFormat From, StorageFormat To, TransferStep Step)[] Edges =
    [
        (StorageFormat.Pk1, StorageFormat.Pk7, TransferStep.VirtualConsole),
        (StorageFormat.Pk2, StorageFormat.Pk7, TransferStep.VirtualConsole),
        (StorageFormat.Pk3, StorageFormat.Pk4, TransferStep.PalPark),
        (StorageFormat.Pk4, StorageFormat.Pk5, TransferStep.PokeTransfer),
        (StorageFormat.Pk5, StorageFormat.Pk6, TransferStep.PokeTransporter),
        (StorageFormat.Pk6, StorageFormat.Pk7, TransferStep.PokemonBank),
        .. HomeTargets.SelectMany(to => new[] { (StorageFormat.Pk7, to, TransferStep.Home), (StorageFormat.Pb7, to, TransferStep.Home) }),
    ];

    /// <summary>Jogos do Switch que recebem do HOME (o Let's Go só manda).</summary>
    private static IEnumerable<StorageFormat> HomeTargets =>
        [StorageFormat.Pk8, StorageFormat.Pb8, StorageFormat.Pa8, StorageFormat.Pk9, StorageFormat.Pa9];

    /// <summary>Formatos pelo caminho, do primeiro depois da origem até o destino; null quando não há caminho.</summary>
    public static IReadOnlyList<(StorageFormat Format, TransferStep Step)>? Path(StorageFormat from, StorageFormat to)
    {
        // O grafo só anda para a frente e tem uma saída por formato até a Gen 7: basta seguir.
        var path = new List<(StorageFormat, TransferStep)>();
        var current = from;
        while (current != to)
        {
            var next = Edges.Where(e => e.From == current).ToList();
            if (next.Count == 0)
                return null;
            var edge = next.FirstOrDefault(e => e.To == to);
            if (edge == default)
            {
                if (next.Count > 1)
                    return null; // no HOME: só o destino pedido
                edge = next[0];
            }
            path.Add((edge.To, edge.Step));
            current = edge.To;
        }
        return path;
    }

    /// <summary>Destinos mais novos que a origem, com o caminho e o motivo de recusa.</summary>
    public static IReadOnlyList<AdaptOption> Options(PKM source, StorageFormat from)
    {
        var rank = VaultFormats.Get(from).Rank;
        var result = new List<AdaptOption>();
        foreach (var target in VaultFormats.All.Where(f => f.Rank > rank))
        {
            if (target.Format == StorageFormat.Pb7)
            {
                result.Add(new AdaptOption(target.Format, [], "lgpe_no_receive", nameof(TransferStep.Home)));
                continue;
            }
            if (Path(from, target.Format) is not { } path)
            {
                result.Add(new AdaptOption(target.Format, [], "no_route", null));
                continue;
            }
            var (refusal, at) = Refusal(source, from, path);
            result.Add(new AdaptOption(target.Format, path.Select(p => p.Step).ToList(), refusal, at));
        }
        return result;
    }

    /// <summary>
    /// Leva o Pokémon até <paramref name="to"/>. <paramref name="owner"/> é quem recebe (o treinador atual nas Gen 6
    /// e 7 e nos jogos do Switch). Notas: "held_item_removed" (o item voltou para a mochila), "moves_replaced" (HOME).
    /// </summary>
    public static HomeTransferResult Adapt(PKM source, StorageFormat from, StorageFormat to, ITrainerInfo owner)
    {
        if (to == StorageFormat.Pb7)
            return new HomeTransferResult(null, "lgpe_no_receive", []);
        if (VaultFormats.Get(to).Rank <= VaultFormats.Get(from).Rank || Path(from, to) is not { } path)
            return new HomeTransferResult(null, "no_route", []);
        if (Refusal(source, from, path).Code is { } refusal)
            return new HomeTransferResult(null, refusal, []);
        try
        {
            return Convert(source, path, owner);
        }
        catch (Exception e) when (e is IndexOutOfRangeException or ArgumentException or InvalidOperationException)
        {
            // Dados que o PKHeX não consegue converter (nomes sem terminador, campos fora da faixa): não há o que levar.
            return new HomeTransferResult(null, "convert_failed", []);
        }
    }

    private static HomeTransferResult Convert(PKM source, IReadOnlyList<(StorageFormat Format, TransferStep Step)> path,
        ITrainerInfo owner)
    {
        var notes = new List<string>();
        var pk = source.Clone();
        foreach (var (format, step) in path)
        {
            if (DropsItem(step, pk) && pk.HeldItem != 0)
            {
                pk.HeldItem = 0;
                if (!notes.Contains("held_item_removed"))
                    notes.Add("held_item_removed");
            }
            PKM? next = (pk, step) switch
            {
                (PK1 gb1, TransferStep.VirtualConsole) => gb1.ConvertToPK7(),
                (PK2 gb2, TransferStep.VirtualConsole) => gb2.ConvertToPK7(),
                (PK3 pk3, TransferStep.PalPark) => pk3.ConvertToPK4(),
                (PK4 pk4, TransferStep.PokeTransfer) => pk4.ConvertToPK5(),
                (PK5 pk5, TransferStep.PokeTransporter) => pk5.ConvertToPK6(),
                (PK6 pk6, TransferStep.PokemonBank) => pk6.ConvertToPK7(),
                (_, TransferStep.Home) => null,
                _ => throw new InvalidOperationException($"Etapa {step} não serve para {pk.GetType().Name}."),
            };
            if (step == TransferStep.Home)
            {
                var game = VaultFormats.Get(format).Game!;
                var moved = HomeTransfer.Move(pk, game, owner);
                if (!moved.Ok)
                    return new HomeTransferResult(null, moved.Refusal, moved.Notes);
                notes.AddRange(moved.Notes);
                return new HomeTransferResult(moved.Pokemon, null, notes);
            }
            pk = next!;
        }

        // Gen 4 a 7 no próprio cofre: quem recebe vira o treinador atual (da Gen 6 em diante, o jogo guarda).
        if (pk is IHandlerUpdate handler)
            handler.UpdateHandler(owner);
        pk.ResetPartyStats();
        pk.RefreshChecksum();
        return new HomeTransferResult(pk, null, notes);
    }

    /// <summary>Etapas que devolvem o item para a mochila (no Virtual Console só a Gen 2: na Gen 1 não há item).</summary>
    private static bool DropsItem(TransferStep step, PKM pk) =>
        step is TransferStep.PokeTransfer or TransferStep.PokeTransporter or TransferStep.PokemonBank or TransferStep.Home
        || (step == TransferStep.VirtualConsole && pk is PK2);

    /// <summary>Primeira recusa pelo caminho (código estável e a etapa).</summary>
    private static (string? Code, string? Step) Refusal(PKM source, StorageFormat from,
        IReadOnlyList<(StorageFormat Format, TransferStep Step)> path)
    {
        if (source.IsEgg)
            return ("egg", path.Count > 0 ? path[0].Step.ToString() : null);
        foreach (var (format, step) in path)
        {
            if (StepRefusal(source, from, step) is { } code)
                return (code, step.ToString());
            if (!VaultFormats.Get(format).HasSpecies(source.Species, source.Form))
                return ("species_not_in_game", step.ToString());
            // Chegou ao Sword/Shield vindo de um jogo que ele não conhece: os dados de origem ficaram no HOME.
            if (step == TransferStep.Home && HomeTransfer.OriginLostInSwsh(source))
                return (HomeTransfer.OriginLostInSwshRefusal, step.ToString());
        }
        return (null, null);
    }

    private static string? StepRefusal(PKM pk, StorageFormat from, TransferStep step) => step switch
    {
        TransferStep.VirtualConsole when pk.Species == Celebi && pk.CurrentLevel < 30 => "vc_celebi",
        TransferStep.VirtualConsole when from == StorageFormat.Pk2 && pk.HasMove(DizzyPunch) => "vc_dizzy_punch",
        TransferStep.VirtualConsole when pk.Species == Mew && !(pk.OriginalTrainerName == "GF" && pk.TID16 == 22796) => "vc_mew_event",
        TransferStep.PalPark when pk.Moves.Any(HiddenMoves3.Contains) => "pal_park_hm",
        TransferStep.PalPark when IsMail3(pk.HeldItem) => "pal_park_mail",
        TransferStep.PokeTransfer when pk.Moves.Any(HiddenMoves4.Contains) => "poke_transfer_hm",
        TransferStep.PokeTransfer when pk.Species == Pichu && pk.Form == 1 => "poke_transfer_spiky_pichu",
        TransferStep.PokeTransporter or TransferStep.PokemonBank when pk.Species == Kyurem && pk.Form != 0 => "kyurem_fused",
        TransferStep.PokemonBank when pk.Species == Pikachu && pk.Form != 0 && from <= StorageFormat.Pk6 => "cosplay_pikachu",
        _ => null,
    };
}
