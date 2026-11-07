using System.Collections.Frozen;
using Microsoft.Extensions.Logging;
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
/// da Gen 3 (<c>EncounterGift1</c>/<c>EncounterGift3*</c>), bônus do Colosseum, Pokémon GO, o Pokéwalker (acessório do
/// HGSS), a Magearna (código QR) e o Pecharunt do SV (só com o item do Mystery Gift).
/// Lendários de um lugar do jogo aberto por item de evento (Mew da Faraway Island, Deoxys, Darkrai, Shaymin, Arceus...)
/// contam: o encontro existe no jogo. Encontro de uma forma anterior conta como <see cref="AvailabilityKind.Evolution"/>.
/// <para>O PKHeX quase nunca marca presente: só o <c>EncounterStatic8</c> tem <c>Gift</c>. Por isso um encontro fixo que não é
/// selvagem nem raid (inicial, presente, lendário parado) fica em <see cref="Statics"/>, abaixo de Gift e Trade: vira
/// Encounter só quando nenhuma outra fonte diz presente ou troca. Encounter direto do PKHeX = há slot selvagem ou raid.</para>
/// <para>Também guarda, por espécie e jogo, se há encontro em que ela pode ser shiny (<see cref="ShinyInfo"/>).</para>
/// <para>Se o cálculo de um jogo falhar, ele fica vazio (registrado uma vez no log) e os outros continuam.</para>
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

    private static readonly Lazy<FrozenDictionary<string, GameTable>> Table =
        new(Build, LazyThreadSafetyMode.ExecutionAndPublication);

    internal sealed record GameTable(FrozenDictionary<int, AvailabilityKind> Kinds, FrozenSet<int> Statics,
        FrozenDictionary<int, ShinyInfo> Shiny)
    {
        public static GameTable Empty { get; } = new(FrozenDictionary<int, AvailabilityKind>.Empty, FrozenSet<int>.Empty,
            FrozenDictionary<int, ShinyInfo>.Empty);
    }

    /// <summary>Log dos jogos cujo cálculo falhou (o app define na abertura; sem ele, nada é registrado).</summary>
    public static ILogger? Logger { get; set; }

    /// <summary>Calcula a tabela em segundo plano (para a primeira ficha da Pokédex não esperar).</summary>
    public static void Warm() => _ = Task.Run(() => Table.Value);

    /// <summary>Jeito mais forte de conseguir a espécie no jogo pelo PKHeX; <see cref="AvailabilityKind.None"/> se nenhum.</summary>
    public static AvailabilityKind Kind(string gameKey, int species) =>
        Table.Value.TryGetValue(gameKey, out var game) ? game.Kinds.GetValueOrDefault(species) : AvailabilityKind.None;

    /// <summary>Espécies obtíveis no jogo pelo PKHeX, com o jeito mais forte.</summary>
    public static IReadOnlyDictionary<int, AvailabilityKind> ForGame(string gameKey) =>
        Table.Value.TryGetValue(gameKey, out var game) ? game.Kinds : FrozenDictionary<int, AvailabilityKind>.Empty;

    /// <summary>Espécies com encontro fixo do PKHeX no jogo que não é selvagem, presente nem troca (veja a classe).</summary>
    public static IReadOnlySet<int> Statics(string gameKey) =>
        Table.Value.TryGetValue(gameKey, out var game) ? game.Statics : FrozenSet<int>.Empty;

    /// <summary>Encontros da espécie no jogo quanto a shiny; nulo quando o PKHeX não tem encontro nenhum dela lá.</summary>
    public static ShinyInfo? Shiny(string gameKey, int species) =>
        Table.Value.TryGetValue(gameKey, out var game) ? game.Shiny.GetValueOrDefault(species) : null;

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

    private static FrozenDictionary<string, GameTable> Build() =>
        Build(key => BuildGame(Games[key].Context, Games[key].Versions), Logger);

    /// <summary>Uma tabela por jogo; o jogo cujo cálculo lança fica vazio (os chamadores tratam como "nenhum").</summary>
    internal static FrozenDictionary<string, GameTable> Build(Func<string, GameTable> buildGame, ILogger? logger) =>
        Games.Keys.ToFrozenDictionary(key => key, key =>
        {
            try
            {
                return buildGame(key);
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Disponibilidade pelo PKHeX: falhou no jogo {Game}; ele fica só com as outras fontes.", key);
                return GameTable.Empty;
            }
        });

    private static GameTable BuildGame(EntityContext context, GameVersion[] versions)
    {
        var personal = PKHeX.Core.GameData.GetPersonal(versions[0]);
        var result = new Dictionary<int, AvailabilityKind>();
        var statics = new HashSet<int>();
        var shiny = new Dictionary<int, ShinyInfo>();
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
                    if (kind == AvailabilityKind.None)
                        continue;
                    if (kind == AvailabilityKind.Encounter && !IsWild(enc))
                        statics.Add(species);
                    else
                        result[species] = Strongest(result.GetValueOrDefault(species), kind);
                    var info = shiny.GetValueOrDefault(species) ?? ShinyInfo.Empty;
                    if (CanBeShiny(enc))
                        info = kind == AvailabilityKind.Evolution ? info with { Evolution = true }
                            : IsWild(enc) ? info with { Wild = true }
                            : info with { Fixed = true };
                    shiny[species] = info;
                }
            }
        }
        return new GameTable(result.ToFrozenDictionary(), statics.ToFrozenSet(), shiny.ToFrozenDictionary());
    }

    /// <summary>
    /// O encontro pode sair shiny? Pela propriedade <see cref="IShinyPotential.Shiny"/> do PKHeX: <see cref="Shiny.Never"/>
    /// é travado e <see cref="Shiny.FixedValue"/> (PID fixo, como nas trocas antigas) também não dá para caçar.
    /// </summary>
    internal static bool CanBeShiny(IEncounterable enc) =>
        enc is not IShinyPotential { Shiny: PKHeX.Core.Shiny.Never or PKHeX.Core.Shiny.FixedValue };

    /// <summary>Selvagem ou raid (onde valem os métodos de campo: Poké Radar, SOS, Max Raid, Tera Raid...).</summary>
    private static bool IsWild(IEncounterable enc)
    {
        var name = enc.GetType().Name;
        return name.StartsWith("EncounterSlot", StringComparison.Ordinal) || name is "EncounterStatic8N" or "EncounterStatic8U"
            or "EncounterTera9";
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
            or "EncounterStatic4Pokewalker"
            || name.StartsWith("EncounterGift3", StringComparison.Ordinal) || name.Contains("Colo", StringComparison.Ordinal)
            || name.Contains("XD", StringComparison.Ordinal) || name.StartsWith("EncounterTransfer", StringComparison.Ordinal))
            return false;
        // A Magearna da Gen 7 só sai pelo código QR do evento; o Pecharunt do SV, só com o item do Mystery Gift.
        return !(enc.Generation == 7 && enc.Species == (ushort)PKHeX.Core.Species.Magearna)
            && !(enc.Context == EntityContext.Gen9 && enc.Species == (ushort)PKHeX.Core.Species.Pecharunt);
    }
}

/// <summary>
/// Encontros de uma espécie num jogo (tabelas do PKHeX) em que ela pode sair shiny: <see cref="Wild"/> (selvagem ou
/// raid), <see cref="Fixed"/> (fixo, presente ou troca) e <see cref="Evolution"/> (uma forma anterior). Tudo falso =
/// todos os encontros são travados (Mew do BDSP, Zacian, iniciais da Gen 8 em diante...).
/// </summary>
public sealed record ShinyInfo(bool Wild, bool Fixed, bool Evolution)
{
    public static ShinyInfo Empty { get; } = new(false, false, false);

    public bool Locked => !Wild && !Fixed && !Evolution;
}
