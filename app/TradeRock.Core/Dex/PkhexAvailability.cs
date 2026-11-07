using System.Collections.Frozen;
using PKHeX.Core;

namespace TradeRock.Core.Dex;

/// <summary>
/// Como conseguir cada espécie em cada jogo segundo as tabelas de encontros do PKHeX.Core (selvagens, fixos, presentes e
/// trocas), que cobrem o que o PokeAPI não tem (iniciais dados de presente, trocas com personagem, lendários
/// fixos). Ovos não entram (veja <see cref="Classify"/>). Calculado uma vez, na primeira consulta (uns poucos segundos), com
/// <see cref="EncounterMovesetGenerator.GenerateEncounters(PKM, ReadOnlyMemory{ushort}, GameVersion[])"/> para cada
/// espécie e forma presente na tabela pessoal do jogo.
/// </summary>
/// <remarks>
/// Fica de fora o que não se consegue só jogando: Mystery Gift (<see cref="MysteryGift"/>), raids e surtos de
/// distribuição do SwSh e do SV, Dream World e Dream Radar (Gen 5), My Pokémon Ranch, presentes de evento da Gen 1 e
/// da Gen 3 (<c>EncounterGift1</c>/<c>EncounterGift3*</c>), bônus do Colosseum, Pokémon GO e os encontros
/// "fatídicos" da Gen 3 e da Gen 4 (só com item de evento: Deoxys, Darkrai, Shaymin, Arceus...) e a Magearna (código
/// QR). Encontro de uma forma anterior (ou ovo dela) conta como <see cref="AvailabilityKind.Evolution"/>.
/// </remarks>
public static class PkhexAvailability
{
    /// <summary>Versões do PKHeX de cada jogo (chaves de <see cref="Games.GameVersions"/>).</summary>
    internal static readonly IReadOnlyDictionary<string, (EntityContext Context, GameVersion[] Versions)> Games =
        new Dictionary<string, (EntityContext, GameVersion[])>
        {
            // Os selvagens do Blue ocidental estão nas tabelas do Green japonês (GN). O RBY (fixos comuns) não entra no
            // Yellow: traria trocas e prêmios que só existem no Red/Blue.
            ["rb"] = (EntityContext.Gen1, [GameVersion.RD, GameVersion.GN, GameVersion.BU]),
            ["y"] = (EntityContext.Gen1, [GameVersion.YW]),
            ["gs"] = (EntityContext.Gen2, [GameVersion.GD, GameVersion.SI]),
            ["c"] = (EntityContext.Gen2, [GameVersion.C]),
            ["rs"] = (EntityContext.Gen3, [GameVersion.R, GameVersion.S]),
            ["e"] = (EntityContext.Gen3, [GameVersion.E]),
            ["frlg"] = (EntityContext.Gen3, [GameVersion.FR, GameVersion.LG]),
            ["dp"] = (EntityContext.Gen4, [GameVersion.D, GameVersion.P]),
            ["pt"] = (EntityContext.Gen4, [GameVersion.Pt]),
            ["hgss"] = (EntityContext.Gen4, [GameVersion.HG, GameVersion.SS]),
            ["bw"] = (EntityContext.Gen5, [GameVersion.B, GameVersion.W]),
            ["b2w2"] = (EntityContext.Gen5, [GameVersion.B2, GameVersion.W2]),
            ["xy"] = (EntityContext.Gen6, [GameVersion.X, GameVersion.Y]),
            ["oras"] = (EntityContext.Gen6, [GameVersion.OR, GameVersion.AS]),
            ["sm"] = (EntityContext.Gen7, [GameVersion.SN, GameVersion.MN]),
            ["usum"] = (EntityContext.Gen7, [GameVersion.US, GameVersion.UM]),
            ["lgpe"] = (EntityContext.Gen7b, [GameVersion.GP, GameVersion.GE]),
            ["swsh"] = (EntityContext.Gen8, [GameVersion.SW, GameVersion.SH]),
            ["bdsp"] = (EntityContext.Gen8b, [GameVersion.BD, GameVersion.SP]),
            ["pla"] = (EntityContext.Gen8a, [GameVersion.PLA]),
            ["sv"] = (EntityContext.Gen9, [GameVersion.SL, GameVersion.VL]),
            ["za"] = (EntityContext.Gen9a, [GameVersion.ZA]),
        };

    private static readonly Lazy<FrozenDictionary<string, FrozenDictionary<int, AvailabilityKind>>> Table =
        new(Build, LazyThreadSafetyMode.ExecutionAndPublication);

    /// <summary>Calcula a tabela em segundo plano (para a primeira ficha da Pokédex não esperar).</summary>
    public static void Warm() => _ = Task.Run(() => Table.Value);

    /// <summary>Jeito mais forte de conseguir a espécie no jogo pelo PKHeX; <see cref="AvailabilityKind.None"/> se nenhum.</summary>
    public static AvailabilityKind Kind(string gameKey, int species) =>
        Table.Value.TryGetValue(gameKey, out var game) ? game.GetValueOrDefault(species) : AvailabilityKind.None;

    /// <summary>Espécies obtíveis no jogo pelo PKHeX, com o jeito mais forte.</summary>
    public static IReadOnlyDictionary<int, AvailabilityKind> ForGame(string gameKey) =>
        Table.Value.TryGetValue(gameKey, out var game) ? game : FrozenDictionary<int, AvailabilityKind>.Empty;

    /// <summary>Ordem de força: Encounter &gt; Gift &gt; Trade &gt; Evolution &gt; Egg &gt; None.</summary>
    public static int Strength(AvailabilityKind kind) => kind switch
    {
        AvailabilityKind.Encounter => 5,
        AvailabilityKind.Gift => 4,
        AvailabilityKind.Trade => 3,
        AvailabilityKind.Evolution => 2,
        AvailabilityKind.Egg => 1,
        _ => 0,
    };

    public static AvailabilityKind Strongest(AvailabilityKind a, AvailabilityKind b) => Strength(b) > Strength(a) ? b : a;

    private static FrozenDictionary<string, FrozenDictionary<int, AvailabilityKind>> Build() =>
        Games.ToFrozenDictionary(g => g.Key, g => BuildGame(g.Value.Context, g.Value.Versions));

    private static FrozenDictionary<int, AvailabilityKind> BuildGame(EntityContext context, GameVersion[] versions)
    {
        var personal = PKHeX.Core.GameData.GetPersonal(versions[0]);
        var result = new Dictionary<int, AvailabilityKind>();
        for (ushort species = 1; species <= personal.MaxSpeciesID; species++)
        {
            var forms = Math.Max((byte)1, personal[species].FormCount);
            for (byte form = 0; form < forms; form++)
            {
                if (!personal.IsPresentInGame(species, form))
                    continue;
                var pk = EntityBlank.GetBlank(context, LanguageID.English);
                pk.Species = species;
                pk.Form = form;
                pk.Language = (int)LanguageID.English;
                pk.SetGender(pk.GetSaneGender());
                foreach (var enc in EncounterMovesetGenerator.GenerateEncounters(pk, ReadOnlyMemory<ushort>.Empty, versions))
                {
                    var kind = Classify(enc, species);
                    if (kind != AvailabilityKind.None)
                        result[species] = Strongest(result.GetValueOrDefault(species), kind);
                }
            }
        }
        return result.ToFrozenDictionary();
    }

    /// <summary>Tipo de um encontro do PKHeX para a espécie pedida; None para o que não se consegue só jogando.</summary>
    internal static AvailabilityKind Classify(IEncounterable enc, ushort species)
    {
        var name = enc.GetType().Name;
        // Os ovos de criação do PKHeX (EncounterEgg*) valem para qualquer espécie da tabela (o pai pode ter vindo de
        // troca): não dizem que dá para chocar só jogando. O Egg fica com a regra da Pokédex (a família tem que existir
        // no jogo). Ovo fixo (Togepi, Riolu, Happiny...) é presente.
        if (enc is MysteryGift || !InGame(enc) || name.StartsWith("EncounterEgg", StringComparison.Ordinal))
            return AvailabilityKind.None;
        if (enc.Species != species)
            return AvailabilityKind.Evolution;
        if (enc.IsEgg)
            return AvailabilityKind.Gift;
        if (name.StartsWith("EncounterTrade", StringComparison.Ordinal))
            return AvailabilityKind.Trade;
        if (name.StartsWith("EncounterGift", StringComparison.Ordinal) || enc is EncounterStatic8 { Gift: true })
            return AvailabilityKind.Gift;
        return AvailabilityKind.Encounter;
    }

    private static bool InGame(IEncounterable enc)
    {
        var name = enc.GetType().Name;
        if (name is "EncounterGift1" or "EncounterGift2" or "EncounterStatic5Entree" or "EncounterStatic5Radar"
            or "EncounterTrade4RanchGift" or "EncounterStatic8NC" or "EncounterStatic8ND" or "EncounterDist9"
            or "EncounterMight9" or "EncounterOutbreak9" or "EncounterSlot7GO" or "EncounterSlot8GO" or "EncounterInvalid"
            || name.StartsWith("EncounterGift3", StringComparison.Ordinal) || name.Contains("Colo", StringComparison.Ordinal)
            || name.Contains("XD", StringComparison.Ordinal) || name.StartsWith("EncounterTransfer", StringComparison.Ordinal)
            || name.StartsWith("EncounterStatic8Nest", StringComparison.Ordinal) && name != "EncounterStatic8N")
            return false;
        // Gen 3 e Gen 4: "fatídico" = só com item de evento (Ilha do Nascimento, Member Card, Oak's Letter...). Na Gen 7,
        // a Magearna só sai pelo código QR do evento.
        if (enc.Generation == 7 && enc.Species == (ushort)PKHeX.Core.Species.Magearna)
            return false;
        // No Platinum, Darkrai, Shaymin e Arceus não vêm marcados como fatídicos, mas também só saem com item de evento.
        if (enc.Generation == 4 && enc.Species is (ushort)PKHeX.Core.Species.Darkrai or (ushort)PKHeX.Core.Species.Shaymin
                or (ushort)PKHeX.Core.Species.Arceus)
            return false;
        return enc.Generation > 4 || enc is not IFatefulEncounterReadOnly { FatefulEncounter: true };
    }
}
