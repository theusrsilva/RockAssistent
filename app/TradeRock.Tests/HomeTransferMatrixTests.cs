using System.Collections.Concurrent;
using PKHeX.Core;
using TradeRock.Core.Builder;
using TradeRock.Core.Games;
using TradeRock.Core.Pokemon;
using TradeRock.Core.Trading;

namespace TradeRock.Tests;

/// <summary>Como o Pokémon de origem de um caso da matriz é obtido.</summary>
internal enum Obtencao
{
    /// <summary>Pelo construtor (<see cref="PokemonBuilder"/>, AutoMod), como a tela "Criar".</summary>
    Construtor,

    /// <summary>Encontro alfa do PKHeX (<see cref="EncounterMovesetGenerator"/> + <see cref="IEncounterConvertible"/>).</summary>
    Alfa,

    /// <summary>Evento do banco embutido no PKHeX (<see cref="EncounterEvent"/>, <see cref="MysteryGift.ConvertToPKM(ITrainerInfo)"/>).</summary>
    Evento,

    /// <summary>Arquivo de demonstração de <c>assets/demo</c>.</summary>
    Arquivo,
}

/// <summary>Um caso da matriz: um Pokémon de origem, de um jogo, obtido de um jeito.</summary>
internal sealed record Caso(string Nome, Obtencao Como, TradeGame Jogo, ushort Especie)
{
    public byte Forma { get; init; }
    public byte Nivel { get; init; } = 50;
    public bool Shiny { get; init; }

    /// <summary>Tipo do encontro pedido ao construtor ("egg", "event"); null = o que o construtor escolher.</summary>
    public string? Encontro { get; init; }

    public ushort Item { get; init; }
    public string? Apelido { get; init; }
    public int? Habilidade { get; init; }
    public string? Genero { get; init; }

    /// <summary>
    /// Forma que se troca dentro do jogo (item, objeto-chave, fusão): o construtor gera a forma normal e o teste põe
    /// <see cref="Forma"/> e <see cref="ItemDaForma"/> (nome em inglês do item que a forma exige segurar, se exigir).
    /// </summary>
    public bool FormaNoJogo { get; init; }
    public string? ItemDaForma { get; init; }

    /// <summary>Número do cartão do evento (<see cref="MysteryGift.CardID"/>) ou nome do arquivo.</summary>
    public int Cartao { get; init; }
    public string? ArquivoDemo { get; init; }

    /// <summary>Shiny travado: o construtor pode recusar (ou entregar sem o shiny); aí o caso não entra na matriz.</summary>
    public bool PodeRecusar { get; init; }

    public GameFamily Familia => GameCatalog.Of(Jogo).Family;
    public override string ToString() => Nome;
}

/// <summary>Gera cada Pokémon de origem uma vez só e guarda para todos os testes da matriz.</summary>
public sealed class HomeTransferMatrixFixture
{
    private readonly ConcurrentDictionary<string, Task<PKM?>> _sources = new();

    internal Task<PKM?> SourceAsync(Caso caso) => _sources.GetOrAdd(caso.Nome, _ => CreateAsync(caso));

    private static async Task<PKM?> CreateAsync(Caso caso)
    {
        var game = GameCatalog.Of(caso.Jogo);
        PKM pk;
        switch (caso.Como)
        {
            case Obtencao.Construtor:
                var built = await BuildAsync(caso);
                if (built is null)
                    return null;
                pk = built;
                break;
            case Obtencao.Alfa:
                pk = Alpha(caso);
                break;
            case Obtencao.Evento:
                pk = Event(caso);
                break;
            default:
                pk = PokemonReader.ToEntity(Pk3Reader.Read(DemoPokemon.All.Single(d => d.Name == caso.ArquivoDemo).Data).Data, game.Family);
                break;
        }
        // Como o cofre guarda: a forma "de caixa" decriptada (o FRLG, os 100 bytes do time).
        if (pk is not PK3)
            pk = PokemonReader.ToEntity(SwitchPokemon.Stored(Prepared(pk)), game.Family);

        var report = LegalityReporter.Analyze(pk, "en");
        Assert.True(report.Status != LegalityStatus.Illegal,
            $"{caso.Nome}: a origem tem que ser legal no próprio jogo: {string.Join(" | ", report.Problems.Select(p => $"{p.Identifier}/{p.Code}"))}");
        Assert.Equal(caso.Especie, pk.Species);
        if (pk is not PK3)
            Assert.Equal(caso.Forma, pk.Form);
        return pk;
    }

    private static PKM Prepared(PKM pk)
    {
        pk.ResetPartyStats();
        pk.RefreshChecksum();
        return pk;
    }

    private static async Task<PKM?> BuildAsync(Caso caso)
    {
        var game = GameCatalog.Of(caso.Jogo);
        string? key = null;
        if (caso.Encontro is { } type)
        {
            // O primeiro encontro desse tipo que dá a própria espécie, no nível pedido ou abaixo.
            var option = BuilderData.Encounters(caso.Especie, caso.Jogo, caso.Forma)
                .FirstOrDefault(e => e.Type == type && e.Species == caso.Especie && e.LevelMin <= caso.Nivel);
            Assert.True(option is not null, $"{caso.Nome}: sem encontro do tipo {type}");
            key = option.Key;
        }
        var spec = new PokemonSpec
        {
            Species = caso.Especie, Form = caso.FormaNoJogo ? (byte)0 : caso.Forma, Level = caso.Nivel, Shiny = caso.Shiny, Game = caso.Jogo,
            EncounterKey = key, HeldItem = caso.Item, Nickname = caso.Apelido, AbilitySlot = caso.Habilidade, Gender = caso.Genero,
            Trainer = game.SixDigitTrainerId ? new TrainerSpec("ROCHA", 123456, 1234) : new TrainerSpec("ROCHA", 24680, 13579),
        };
        var result = await PokemonBuilder.GenerateAsync(spec, "en", default);
        // Shiny travado: o construtor não entrega ("failed") ou entrega sem o shiny, com a diferença avisada.
        if (caso.PodeRecusar && (!result.Usable || result.Differences.Any(d => d.Field == "shiny")))
            return null;
        Assert.True(result.Usable,
            $"{caso.Nome}: {result.Generation} {string.Join(",", result.Errors)} {string.Join(" | ", result.Legality?.Problems.Select(p => $"{p.Identifier}/{p.Code}") ?? [])}");
        var pk = PokemonReader.ToEntity(result.Data!, game.Family);
        Assert.Equal(caso.Shiny, pk.IsShiny);
        Assert.Equal(caso.Nivel, pk.CurrentLevel);
        if (caso.Item != 0)
            Assert.Equal(caso.Item, pk.HeldItem);
        if (caso.Apelido is not null)
            Assert.Equal(caso.Apelido, pk.Nickname);
        if (caso.Habilidade is { } slot)
            Assert.Equal(slot == 2 ? 4 : slot + 1, pk.AbilityNumber);
        if (caso.Encontro == "egg")
            Assert.True(pk is PK3 ? pk.MetLevel == 0 : pk.EggLocation != 0, caso.Nome); // a Gen 3 não guarda o local do ovo
        if (caso.FormaNoJogo)
        {
            pk.Form = caso.Forma;
            pk.RefreshAbility(pk.AbilityNumber >> 1);
            if (pk is PA8 pa8)
            {
                // No Arceus a altura e o peso gravados são os da forma.
                pa8.ResetHeight();
                pa8.ResetWeight();
            }
            if (caso.ItemDaForma is { } item)
            {
                var id = Array.IndexOf(BuilderData.Strings.itemlist, item);
                Assert.True(id > 0, $"{caso.Nome}: item {item}");
                pk.HeldItem = id;
            }
        }
        if (caso.Encontro == "event")
            Assert.True(pk.FatefulEncounter || pk.MetLocation == BuilderData.EventLocation);
        return pk;
    }

    /// <summary>
    /// Alfa: o <see cref="PokemonSpec"/> não tem o campo, então o encontro vem direto do PKHeX: o primeiro encontro alfa
    /// da própria espécie, convertido com o treinador de teste (<see cref="IEncounterConvertible"/>).
    /// </summary>
    private static PKM Alpha(Caso caso)
    {
        AutoModEnvironment.EnsureInitialized();
        var game = GameCatalog.Of(caso.Jogo);
        var version = GameCatalog.ToVersion(caso.Jogo);
        var blank = game.Blank();
        blank.Species = caso.Especie;
        blank.Form = caso.Forma;
        blank.Language = (int)LanguageID.English;
        blank.SetGender(blank.GetSaneGender());
        var encounter = EncounterMovesetGenerator.GenerateEncounters(blank, ReadOnlyMemory<ushort>.Empty, version)
            .First(e => e.Species == caso.Especie && e.Form == caso.Forma && e is IAlphaReadOnly { IsAlpha: true });
        var criteria = EncounterCriteria.Unrestricted with { Shiny = caso.Shiny ? Shiny.Always : Shiny.Never };
        var pk = ((IEncounterConvertible)encounter).ConvertToPKM(Trainer(version), criteria);
        Assert.True(pk is IAlphaReadOnly { IsAlpha: true }, caso.Nome);
        Assert.Equal(caso.Shiny, pk.IsShiny);
        return pk;
    }

    /// <summary>Evento do banco do PKHeX, recebido pelo treinador de teste (o evento põe o treinador dele, se tiver).</summary>
    private static PKM Event(Caso caso)
    {
        AutoModEnvironment.EnsureInitialized();
        var game = GameCatalog.Of(caso.Jogo);
        MysteryGift[] db = game.Family switch
        {
            GameFamily.Lgpe => EncounterEvent.MGDB_G7GG,
            GameFamily.Swsh => EncounterEvent.MGDB_G8,
            GameFamily.Bdsp => EncounterEvent.MGDB_G8B,
            GameFamily.Pla => EncounterEvent.MGDB_G8A,
            GameFamily.Sv => EncounterEvent.MGDB_G9,
            GameFamily.Za => EncounterEvent.MGDB_G9A,
            _ => throw new ArgumentOutOfRangeException(nameof(caso)),
        };
        var gift = db.First(g => g.IsEntity && g.CardID == caso.Cartao && g.Species == caso.Especie && g.Form == caso.Forma);
        var versions = game.Versions.Select(GameCatalog.ToVersion).ToList();
        var pk = gift.ConvertToPKM(Trainer(versions.Contains(gift.Version) ? gift.Version : versions[0]));
        Assert.True(pk.FatefulEncounter, caso.Nome);
        Assert.Equal(caso.Shiny, pk.IsShiny);
        return pk;
    }

    private static SimpleTrainerInfo Trainer(GameVersion version) =>
        new(version) { OT = "ROCHA", TID16 = 0xE240, SID16 = 1, Language = (int)LanguageID.English };
}

/// <summary>
/// A matriz ampliada de "Mover para outro jogo" (D22) e de "Copiar para ser válido no jogo" (D36): shiny, alfa,
/// eventos, formas e variações simples, de todas as origens para todos os destinos, em uma e em duas pernas.
/// A regra: (1) a recusa de <see cref="HomeTransfer.Destinations"/> é a de <see cref="HomeTransfer.Move"/>; (2) nada
/// lança exceção; (3) todo mover aceito sai legal ou "não verificável", nunca ilegal, com a mesma espécie e o mesmo
/// shiny. A matriz base (três espécies comuns) fica em <see cref="HomeTransferTests"/>.
/// </summary>
public sealed class HomeTransferMatrixTests(HomeTransferMatrixFixture fixture) : IClassFixture<HomeTransferMatrixFixture>
{
    private const ushort Leftovers = 234;
    private const ushort LeftoversGen3 = 200;

    internal static readonly Caso[] Casos = BuildCases();

    private static Caso[] BuildCases()
    {
        var list = new List<Caso>();
        Caso C(string name, TradeGame game, ushort species, byte form = 0) => new(name, Obtencao.Construtor, game, species) { Forma = form };
        Caso E(string name, TradeGame game, int card, ushort species, byte form = 0, bool shiny = false) =>
            new(name, Obtencao.Evento, game, species) { Forma = form, Cartao = card, Shiny = shiny };

        // Variações simples e shiny, com o Eevee (existe nos sete jogos).
        (TradeGame Game, string Tag, bool Eggs, bool Items, bool Hidden)[] games =
        [
            (TradeGame.FireRed, "frlg", true, true, false),
            (TradeGame.LetsGoPikachu, "lgpe", false, false, false),
            (TradeGame.Sword, "swsh", true, true, true),
            (TradeGame.BrilliantDiamond, "bdsp", true, true, true),
            (TradeGame.LegendsArceus, "pla", false, false, false),
            (TradeGame.Scarlet, "sv", true, true, true),
            (TradeGame.LegendsZA, "za", false, true, false),
        ];
        foreach (var (game, tag, eggs, items, hidden) in games)
        {
            var gen3 = game == TradeGame.FireRed;
            list.Add(C($"{tag}-eevee-shiny", game, 133) with { Shiny = true });
            list.Add(C($"{tag}-eevee-nivel-100", game, 133) with { Nivel = 100 });
            list.Add(C($"{tag}-eevee-apelido", game, 133) with { Apelido = "Faisca" });
            if (eggs)
            {
                // Nascido de ovo: nível 1 (5 na Gen 3) e já crescido.
                list.Add(C($"{tag}-eevee-ovo-nivel-minimo", game, 133) with { Encontro = "egg", Nivel = (byte)(gen3 ? 5 : 1) });
                list.Add(C($"{tag}-eevee-ovo-shiny", game, 133) with { Encontro = "egg", Shiny = true });
            }
            if (items)
                list.Add(C($"{tag}-eevee-item", game, 133) with { Item = gen3 ? LeftoversGen3 : Leftovers });
            if (hidden)
                list.Add(C($"{tag}-eevee-habilidade-oculta", game, 133) with { Habilidade = 2 });
        }
        // Nível 1 sem ovo: só por evento (Mew do Poké Ball Plus, abaixo).

        // Shiny travado: o construtor recusa na origem (ou acha um evento shiny de verdade, que aí entra na matriz).
        foreach (var (tag, game, species) in new (string, TradeGame, ushort)[]
                 {
                     ("swsh-zacian", TradeGame.Sword, 888), ("swsh-kubfu", TradeGame.Sword, 891), ("bdsp-arceus", TradeGame.BrilliantDiamond, 493),
                     ("pla-dialga", TradeGame.LegendsArceus, 483), ("sv-koraidon", TradeGame.Scarlet, 1007), ("sv-ogerpon", TradeGame.Scarlet, 1017),
                     ("za-zygarde", TradeGame.LegendsZA, 718), ("lgpe-mew", TradeGame.LetsGoPikachu, 151),
                 })
        {
            list.Add(C($"{tag}-shiny-travado", game, species) with { Shiny = true, Nivel = 100, PodeRecusar = true });
        }

        // Alfa (Legends: Arceus e Legends: Z-A).
        foreach (var (tag, game) in new[] { ("pla", TradeGame.LegendsArceus), ("za", TradeGame.LegendsZA) })
        {
            foreach (var species in new ushort[] { 25, 130 })
            {
                list.Add(new Caso($"{tag}-alfa-{species}", Obtencao.Alfa, game, species) );
                list.Add(new Caso($"{tag}-alfa-shiny-{species}", Obtencao.Alfa, game, species) { Shiny = true });
            }
        }

        // Eventos.
        list.Add(C("frlg-evento-jirachi", TradeGame.FireRed, 385) with { Encontro = "event", Nivel = 5 });
        list.Add(C("frlg-evento-mew", TradeGame.FireRed, 151) with { Encontro = "event", Nivel = 10 });
        list.Add(new Caso("frlg-demo-mewtwo", Obtencao.Arquivo, TradeGame.FireRed, 150) { ArquivoDemo = "mewtwo.pk3" });
        list.Add(new Caso("frlg-demo-deoxys", Obtencao.Arquivo, TradeGame.FireRed, 386) { ArquivoDemo = "deoxys.pk3" });
        list.Add(E("lgpe-evento-pikachu-shiny", TradeGame.LetsGoPikachu, 501, 25, shiny: true));
        list.Add(E("lgpe-evento-mew-nivel-1", TradeGame.LetsGoPikachu, 2001, 151));
        list.Add(E("lgpe-evento-mewtwo", TradeGame.LetsGoPikachu, 2, 150));
        list.Add(E("lgpe-evento-melmetal", TradeGame.LetsGoPikachu, 1502, 809));
        list.Add(E("swsh-evento-pikachu-bone", TradeGame.Sword, 1634, 25, 9));
        list.Add(E("swsh-evento-zacian-shiny", TradeGame.Sword, 1639, 888, shiny: true));
        list.Add(E("swsh-evento-celebi-shiny", TradeGame.Sword, 108, 251, shiny: true));
        list.Add(E("swsh-evento-zarude-dada", TradeGame.Sword, 110, 893, 1));
        list.Add(E("swsh-evento-moltres-galar-shiny", TradeGame.Sword, 1, 146, 1, shiny: true));
        list.Add(E("swsh-evento-mew-nivel-1", TradeGame.Sword, 2001, 151));
        list.Add(E("swsh-evento-meowth-gigantamax", TradeGame.Sword, 1601, 52));
        list.Add(E("swsh-evento-jirachi", TradeGame.Sword, 143, 385));
        list.Add(E("swsh-evento-zeraora-home", TradeGame.Sword, 9011, 807, shiny: true));
        list.Add(E("bdsp-evento-piplup", TradeGame.BrilliantDiamond, 203, 393));
        list.Add(E("bdsp-evento-regigigas", TradeGame.BrilliantDiamond, 1201, 486));
        list.Add(E("bdsp-evento-manaphy-home", TradeGame.BrilliantDiamond, 9026, 490, shiny: true));
        list.Add(E("bdsp-evento-turtwig-home", TradeGame.BrilliantDiamond, 9015, 387));
        list.Add(E("pla-evento-growlithe-hisui", TradeGame.LegendsArceus, 801, 58, 1));
        list.Add(E("pla-evento-clefairy", TradeGame.LegendsArceus, 151, 35));
        list.Add(E("pla-evento-regigigas", TradeGame.LegendsArceus, 1201, 486));
        list.Add(E("pla-evento-enamorus-home", TradeGame.LegendsArceus, 9027, 905, shiny: true));
        list.Add(E("sv-evento-lucario-shiny", TradeGame.Scarlet, 1530, 448, shiny: true));
        list.Add(E("sv-evento-miraidon-shiny", TradeGame.Scarlet, 1540, 1008, shiny: true));
        list.Add(E("sv-evento-typhlosion-hisui", TradeGame.Scarlet, 527, 157, 1));
        list.Add(E("sv-evento-zoroark-hisui", TradeGame.Scarlet, 1513, 571, 1));
        list.Add(E("sv-evento-mew", TradeGame.Scarlet, 1521, 151));
        list.Add(E("sv-evento-deoxys", TradeGame.Scarlet, 56, 386));
        list.Add(E("sv-evento-tatsugiri", TradeGame.Scarlet, 505, 978, 2));
        list.Add(E("sv-evento-meloetta-home", TradeGame.Scarlet, 9024, 648, shiny: true));
        list.Add(E("za-evento-ralts", TradeGame.LegendsZA, 1601, 280));
        list.Add(E("za-evento-charizard", TradeGame.LegendsZA, 1607, 6));
        list.Add(E("za-evento-slowpoke", TradeGame.LegendsZA, 102, 79));
        list.Add(E("za-evento-volcanion-home", TradeGame.LegendsZA, 9034, 721, shiny: true));

        // Formas que o construtor gera: regionais, as que só existem em alguns jogos e as de gênero.
        foreach (var (game, tag, forms) in new (TradeGame, string, (ushort Species, byte Form)[])[]
                 {
                     (TradeGame.FireRed, "frlg", [(201, 0), (351, 0)]),
                     (TradeGame.LetsGoPikachu, "lgpe", [(26, 1), (37, 1), (105, 1)]),
                     (TradeGame.Sword, "swsh", [(77, 1), (52, 2), (79, 1), (479, 1), (37, 1), (849, 1), (892, 1), (869, 1), (550, 1)]),
                     (TradeGame.BrilliantDiamond, "bdsp", [(479, 2), (201, 3), (422, 1), (413, 1)]),
                     (TradeGame.LegendsArceus, "pla", [(58, 1), (570, 1), (157, 1), (550, 2), (215, 1), (37, 1), (628, 1), (479, 3), (201, 1)]),
                     (TradeGame.Scarlet, "sv", [(194, 1), (128, 1), (128, 2), (58, 1), (52, 2), (52, 1), (745, 2), (741, 1), (550, 2), (931, 1),
                         (978, 1), (982, 1), (925, 1), (479, 1), (79, 1)]),
                     (TradeGame.LegendsZA, "za", [(676, 1), (710, 1), (26, 1), (79, 1), (669, 1), (618, 1)]),
                 })
        {
            foreach (var (species, form) in forms)
                list.Add(C($"{tag}-forma-{species}-{form}", game, species, form));
        }
        // Encontros fixos (estáticos e presentes do jogo), que o PKHeX confere com mais rigor que os selvagens.
        list.Add(C("pla-fixo-cyndaquil", TradeGame.LegendsArceus, 155) with { Nivel = 5 });
        list.Add(C("pla-fixo-giratina", TradeGame.LegendsArceus, 487) with { Nivel = 70 });
        list.Add(C("swsh-fixo-giratina-max-lair", TradeGame.Sword, 487) with { Nivel = 70 });
        list.Add(C("bdsp-fixo-giratina", TradeGame.BrilliantDiamond, 487) with { Nivel = 70 });
        list.Add(C("sv-fixo-koraidon", TradeGame.Scarlet, 1007) with { Nivel = 72 });
        list.Add(C("za-aegislash", TradeGame.LegendsZA, 681) with { Nivel = 70 });
        list.Add(C("swsh-forma-indeedee-femea", TradeGame.Sword, 876, 1) with { Genero = "F" });
        list.Add(C("za-forma-meowstic-femea", TradeGame.LegendsZA, 678, 1) with { Genero = "F" });

        // Formas trocadas dentro do jogo (item segurado, objeto-chave, fusão): as que têm regra na transferência.
        foreach (var (game, tag, species, form, item) in new (TradeGame, string, ushort, byte, string?)[]
                 {
                     (TradeGame.Sword, "swsh", 487, 1, "Griseous Orb"), (TradeGame.Sword, "swsh", 773, 1, "Fighting Memory"),
                     (TradeGame.Sword, "swsh", 646, 1, null), (TradeGame.Sword, "swsh", 800, 1, null),
                     (TradeGame.BrilliantDiamond, "bdsp", 487, 1, "Griseous Orb"), (TradeGame.BrilliantDiamond, "bdsp", 492, 1, null),
                     (TradeGame.LegendsArceus, "pla", 487, 1, null), (TradeGame.LegendsArceus, "pla", 483, 1, null),
                     (TradeGame.LegendsArceus, "pla", 492, 1, null), (TradeGame.LegendsArceus, "pla", 641, 1, null),
                     (TradeGame.LegendsArceus, "pla", 905, 1, null),
                     (TradeGame.LegendsZA, "za", 681, 1, null),
                 })
        {
            list.Add(C($"{tag}-forma-no-jogo-{species}-{form}", game, species, form) with { FormaNoJogo = true, ItemDaForma = item, Nivel = 70 });
        }
        return [.. list];
    }

    public static TheoryData<string> Nomes() => [.. Casos.Select(c => c.Nome)];

    private static Caso Get(string name) => Casos.Single(c => c.Nome == name);

    /// <summary>O "Meu treinador" do jogo de destino: outro treinador, ou o próprio dono do Pokémon.</summary>
    private static SimpleTrainerInfo Owner(SwitchGame to, PKM? same)
    {
        var version = GameCatalog.ToVersion(to.Versions[0]);
        return same is null
            ? new SimpleTrainerInfo(version) { OT = "Vizinha", TID16 = 0x1234, SID16 = 0x0002, Gender = 1, Language = (int)LanguageID.English }
            : new SimpleTrainerInfo(version)
            {
                OT = same.OriginalTrainerName, TID16 = same.TID16, SID16 = same.SID16, Gender = same.OriginalTrainerGender, Language = same.Language,
            };
    }

    /// <param name="Path">O caminho até a recusa, como "caso [outro]: Sv > Pla > Sv".</param>
    internal sealed record Recusa(PKM Source, GameFamily From, GameFamily To, string Code, string Path)
    {
        public string Route => Path.Replace(" [outro]", "").Replace(" [dono]", "");
    }

    /// <summary>
    /// Confere uma perna: destinos e mover concordam, sem exceção, e o que sai não é ilegal. Devolve o que foi movido.
    /// </summary>
    private static List<(PKM Pokemon, GameFamily Family)> Agree(
        PKM source, GameFamily from, bool sameOwner, string path, List<string> violations, List<Recusa>? refusals, ref int moves)
    {
        var moved = new List<(PKM, GameFamily)>();
        IReadOnlyDictionary<GameFamily, string?> destinations;
        try
        {
            destinations = HomeTransfer.Destinations(source, from);
        }
        catch (Exception e)
        {
            violations.Add($"{path}: Destinations lançou {e.GetType().Name}: {e.Message}");
            return moved;
        }
        if (destinations[from] != "same_game")
            violations.Add($"{path}: o próprio jogo tem que dar same_game, deu {destinations[from] ?? "null"}");

        foreach (var game in GameCatalog.All.Where(g => g.Family != from))
        {
            var route = $"{path} > {game.Family}";
            moves++;
            HomeTransferResult result;
            try
            {
                result = HomeTransfer.Move(source, game, Owner(game, sameOwner ? source : null));
            }
            catch (Exception e)
            {
                violations.Add($"{route}: Move lançou {e.GetType().Name}: {e.Message}");
                continue;
            }
            if (result.Refusal != destinations[game.Family])
                violations.Add($"{route}: destinos={destinations[game.Family] ?? "pode"} mover={result.Refusal ?? "ok"}");
            if (!result.Ok)
            {
                if (result.Pokemon is not null || result.Refusal is null)
                    violations.Add($"{route}: recusa sem código ou com Pokémon");
                else
                    refusals?.Add(new Recusa(source, from, game.Family, result.Refusal, route));
                continue;
            }
            var pk = result.Pokemon!;
            if (pk.GetType() != game.EntityType)
                violations.Add($"{route}: saiu {pk.GetType().Name}");
            if (pk.Species != source.Species)
                violations.Add($"{route}: espécie {source.Species} virou {pk.Species}");
            if (pk.IsShiny != source.IsShiny)
                violations.Add($"{route}: shiny {source.IsShiny} virou {pk.IsShiny}");
            if (!game.HasSpecies(pk.Species, pk.Form))
                violations.Add($"{route}: forma {pk.Species}-{pk.Form} não existe no destino");
            try
            {
                var report = LegalityReporter.Analyze(pk, "en");
                if (report.Status == LegalityStatus.Illegal)
                    violations.Add($"{route}: ILEGAL {string.Join(" | ", report.Problems.Select(p => $"{p.Identifier}/{p.Code}"))}");
            }
            catch (Exception e)
            {
                violations.Add($"{route}: Analyze lançou {e.GetType().Name}: {e.Message}");
            }
            moved.Add((pk, game.Family));
        }
        return moved;
    }

    /// <summary>As duas pernas de um caso, com o outro treinador e com o próprio dono.</summary>
    private static (List<string> Violations, List<Recusa> Refusals, int Moves) Run(Caso caso, PKM source)
    {
        var violations = new List<string>();
        var refusals = new List<Recusa>();
        var moves = 0;
        foreach (var same in new[] { false, true })
        {
            var who = same ? "dono" : "outro";
            // As recusas não dependem do treinador: guardam-se as de uma rodada só.
            var keep = same ? null : refusals;
            foreach (var first in Agree(source, caso.Familia, same, $"{caso.Nome} [{who}]: {caso.Familia}", violations, keep, ref moves))
                Agree(first.Pokemon, first.Family, same, $"{caso.Nome} [{who}]: {caso.Familia} > {first.Family}", violations, keep, ref moves);
        }
        return (violations, refusals, moves);
    }

    [Theory]
    [MemberData(nameof(Nomes))]
    public async Task Destinos_e_mover_concordam_e_nada_sai_ilegal(string nome)
    {
        var caso = Get(nome);
        var source = await fixture.SourceAsync(caso);
        if (source is null)
        {
            Assert.True(caso.PodeRecusar); // shiny travado: o construtor recusou na origem
            return;
        }
        var (violations, _, moves) = Run(caso, source);
        Assert.True(moves > 0);
        Assert.True(violations.Count == 0, string.Join("\n", violations));
    }

    /// <summary>Recusas do mover em que a tela oferece (ou pode oferecer) a cópia nascida no jogo de destino.</summary>
    private static readonly string[] CopyRefusals =
        [HomeTransfer.OriginNotAccepted, "no_route", HomeTransfer.OriginLostInSwshRefusal, HomeTransfer.IllegalAfterMove];

    private static readonly string[] StableCopyRefusals = ["same_game", "egg", "species_not_in_game", "copy_not_possible"];

    /// <summary>
    /// Onde o mover recusa pela origem, pela falta de caminho, pela origem perdida ou porque sairia inválido, a cópia (D36) ou sai aceita
    /// (legal ou "não verificável", nascida no jogo de destino, com a espécie, a forma, o shiny e o nível do original)
    /// ou é recusada com código estável. Nunca exceção, nunca ilegal.
    /// </summary>
    [Theory]
    [MemberData(nameof(Nomes))]
    public async Task Copia_onde_o_mover_recusa_sai_valida_ou_recusa_com_codigo(string nome)
    {
        var caso = Get(nome);
        var source = await fixture.SourceAsync(caso);
        if (source is null)
        {
            Assert.True(caso.PodeRecusar);
            return;
        }
        var (_, refusals, _) = Run(caso, source);
        var violations = new List<string>();
        var trainer = new TrainerSpec("Treinadora", 654321, 1234, 1);
        foreach (var refusal in refusals.Where(r => CopyRefusals.Contains(r.Code)))
        {
            var to = GameCatalog.Get(refusal.To);
            var route = $"{refusal.Path} ({refusal.Code})";
            GameCopyResult copy;
            try
            {
                copy = await GameCopy.CreateAsync(refusal.Source, refusal.From, to, trainer, "en", default);
            }
            catch (Exception e)
            {
                violations.Add($"{route}: a cópia lançou {e.GetType().Name}: {e.Message}");
                continue;
            }
            if (!copy.Ok)
            {
                if (copy.Pokemon is not null || !StableCopyRefusals.Contains(copy.Refusal))
                    violations.Add($"{route}: recusa da cópia sem código estável: {copy.Refusal ?? "null"}");
                else if (copy.Refusal == "copy_not_possible" && string.IsNullOrEmpty(copy.Reason))
                    violations.Add($"{route}: copy_not_possible sem motivo");
                continue;
            }
            var pk = copy.Pokemon!;
            if (pk.GetType() != to.EntityType)
                violations.Add($"{route}: a cópia saiu {pk.GetType().Name}");
            if (!to.Versions.Select(GameCatalog.ToVersion).Contains(pk.Version))
                violations.Add($"{route}: a cópia nasceu em {pk.Version}");
            if (pk.Species != refusal.Source.Species || pk.Form != refusal.Source.Form)
                violations.Add($"{route}: a cópia saiu {pk.Species}-{pk.Form}, o original é {refusal.Source.Species}-{refusal.Source.Form}");
            if (pk.IsShiny != refusal.Source.IsShiny)
                violations.Add($"{route}: shiny {refusal.Source.IsShiny} virou {pk.IsShiny}");
            if (pk.CurrentLevel != refusal.Source.CurrentLevel || pk.MetLevel > pk.CurrentLevel)
                violations.Add($"{route}: nível {refusal.Source.CurrentLevel} virou {pk.CurrentLevel} (encontro no {pk.MetLevel})");
            if (pk.OriginalTrainerName != trainer.Name)
                violations.Add($"{route}: treinador {pk.OriginalTrainerName}");
            try
            {
                var report = LegalityReporter.Analyze(pk, "en");
                if (report.Status == LegalityStatus.Illegal)
                    violations.Add($"{route}: cópia ILEGAL {string.Join(" | ", report.Problems.Select(p => $"{p.Identifier}/{p.Code}"))}");
            }
            catch (Exception e)
            {
                violations.Add($"{route}: Analyze lançou {e.GetType().Name}: {e.Message}");
            }
        }
        Assert.True(violations.Count == 0, string.Join("\n", violations));
    }

    private async Task<PKM> SourceAsync(string name) => (await fixture.SourceAsync(Get(name)))!;

    private static HomeTransferResult Move(PKM pk, GameFamily to) => HomeTransfer.Move(pk, GameCatalog.Get(to), Owner(GameCatalog.Get(to), null));

    private static void NotIllegal(PKM pk)
    {
        var report = LegalityReporter.Analyze(pk, "en");
        Assert.True(report.Status != LegalityStatus.Illegal, string.Join(" | ", report.Problems.Select(p => $"{p.Identifier}/{p.Code}")));
    }

    /// <summary>
    /// Shiny travado: o construtor recusa na origem, e o caso nem entra na matriz. Onde existe um shiny de verdade
    /// (evento do Zacian e do Koraidon, Arceus do BDSP), o construtor entrega e a matriz confere como os outros.
    /// </summary>
    [Theory]
    [InlineData("swsh-kubfu-shiny-travado", false)]
    [InlineData("pla-dialga-shiny-travado", false)]
    [InlineData("sv-ogerpon-shiny-travado", false)]
    [InlineData("za-zygarde-shiny-travado", false)]
    [InlineData("lgpe-mew-shiny-travado", false)]
    [InlineData("swsh-zacian-shiny-travado", true)]
    [InlineData("sv-koraidon-shiny-travado", true)]
    [InlineData("bdsp-arceus-shiny-travado", true)]
    public async Task Shiny_travado_o_construtor_recusa_na_origem(string nome, bool existe)
    {
        var source = await fixture.SourceAsync(Get(nome));
        Assert.Equal(existe, source is not null);
        if (source is not null)
            Assert.True(source.IsShiny);
    }

    /// <summary>
    /// Defeito achado pela matriz: a conversão devolve o item à mochila (como o HOME), e a Giratina na forma Origem
    /// chegava sem a Griseous Orb, ilegal (FormItemInvalid). Agora volta à forma Alterada, como no jogo. No Legends:
    /// Arceus a forma não depende de item, então fica.
    /// </summary>
    [Theory]
    [InlineData("swsh-forma-no-jogo-487-1", GameFamily.Bdsp)]
    [InlineData("swsh-forma-no-jogo-487-1", GameFamily.Sv)]
    [InlineData("bdsp-forma-no-jogo-487-1", GameFamily.Swsh)]
    [InlineData("pla-forma-no-jogo-487-1", GameFamily.Swsh)]
    [InlineData("pla-forma-no-jogo-483-1", GameFamily.Sv)]
    public async Task Forma_que_depende_do_item_volta_a_normal_sem_o_item(string nome, GameFamily destino)
    {
        var source = await SourceAsync(nome);
        Assert.Equal(1, source.Form);

        // Com outro treinador e com o próprio dono (no Sword/Shield, o dono também vira o treinador atual).
        foreach (var owner in new[] { Owner(GameCatalog.Get(destino), null), Owner(GameCatalog.Get(destino), source) })
        {
            var moved = HomeTransfer.Move(source, GameCatalog.Get(destino), owner);
            Assert.True(moved.Ok, moved.Refusal);
            Assert.Equal(0, moved.Pokemon!.Form);
            Assert.Equal(0, moved.Pokemon.HeldItem);
            Assert.Contains("form_reverted", moved.Notes);
            // A habilidade é a da forma nova (a Giratina Origem tem Levitate; a Alterada, Pressure).
            var slot = moved.Pokemon.AbilityNumber >> 1;
            Assert.Equal(GameCatalog.Get(destino).PersonalTable.GetFormEntry(source.Species, 0).GetAbilityAtIndex(slot), moved.Pokemon.Ability);
            NotIllegal(moved.Pokemon);
        }

        if (Get(nome).Familia != GameFamily.Pla)
        {
            var inArceus = Move(source, GameFamily.Pla);
            Assert.True(inArceus.Ok, inArceus.Refusal);
            Assert.Equal(1, inArceus.Pokemon!.Form);
            Assert.DoesNotContain("form_reverted", inArceus.Notes);
            NotIllegal(inArceus.Pokemon);
        }
    }

    /// <summary>
    /// Defeito achado pela matriz: o BDSP e o Arceus não guardam os golpes de "relembrar" do Scarlet/Violet, e o Lucario
    /// shiny de evento voltava ao Scarlet/Violet sem eles: ilegal, e sem nenhum motivo na lista. Agora o mover refaz os
    /// golpes de relembrar (são os do evento), e a análise passou a listar esse problema.
    /// </summary>
    [Theory]
    [InlineData(GameFamily.Bdsp)]
    [InlineData(GameFamily.Pla)]
    public async Task Evento_que_vai_e_volta_recupera_os_golpes_de_relembrar(GameFamily meio)
    {
        var source = await SourceAsync("sv-evento-lucario-shiny");
        ushort[] relearn = [source.RelearnMove1, source.RelearnMove2, source.RelearnMove3, source.RelearnMove4];
        Assert.Contains(relearn, m => m != 0);

        var away = Move(source, meio);
        Assert.True(away.Ok, away.Refusal);
        // A perda é da conversão do PKHeX: o formato do meio fica sem os golpes de relembrar, e a volta crua é ilegal.
        var raw = EntityConverter.ConvertToType(away.Pokemon!.Clone(), typeof(PK9), out _);
        Assert.NotNull(raw);
        Assert.Equal(0, raw.RelearnMove1);
        var report = LegalityReporter.Analyze(raw, "en");
        Assert.Equal(LegalityStatus.Illegal, report.Status);
        Assert.Contains(report.Problems, p => p.Identifier == "RelearnMove" && p.Field == "moves");

        var back = Move(away.Pokemon, GameFamily.Sv);
        Assert.True(back.Ok, back.Refusal);
        Assert.Equal(relearn, new[] { back.Pokemon!.RelearnMove1, back.Pokemon.RelearnMove2, back.Pokemon.RelearnMove3, back.Pokemon.RelearnMove4 });
        Assert.True(back.Pokemon.IsShiny);
        NotIllegal(back.Pokemon);
    }

    /// <summary>
    /// Defeito achado pela matriz: Pokémon válido que saía ILEGAL do mover, porque o formato por onde passou não guarda
    /// um dado que o jogo de origem confere (o HOME de verdade guarda do lado dele) ou porque o PKHeX não reconhece a
    /// origem no destino. Não há o que refazer sem inventar o dado, então o mover recusa com "illegal_after_move".
    /// A lista é a de todos os casos da matriz (com o PKHeX.Core 26.8.26); o que cada um perde:
    /// - Sword/Shield de raid ou Max Lair que foi a outro jogo: o nível Dynamax (o encontro exige um mínimo);
    /// - evento do Sword/Shield que foi ao Z-A: nível Dynamax e altura/peso;
    /// - evento do Arceus que foi ao BDSP ou ao Scarlet/Violet: os "effort levels" (GV) e a bola do evento;
    /// - evento do Scarlet/Violet que foi ao Arceus: o Tera Type do evento;
    /// - Typhlosion de Hisui nascido no Arceus (inicial evoluído): o PKHeX não acha o encontro dele no Scarlet/Violet.
    /// </summary>
    [Fact]
    public async Task Valido_que_sairia_invalido_e_recusado_com_codigo()
    {
        string[] expected =
        [
            "pla-evento-enamorus-home: Pla > Sv > Pla",
            "pla-evento-regigigas: Pla > Bdsp > Pla",
            "pla-evento-regigigas: Pla > Sv > Pla",
            "pla-forma-157-1: Pla > Sv",
            "sv-evento-zoroark-hisui: Sv > Pla > Sv",
            "swsh-evento-zeraora-home: Swsh > Za > Swsh",
            "swsh-fixo-giratina-max-lair: Swsh > Bdsp > Swsh",
            "swsh-fixo-giratina-max-lair: Swsh > Pla > Swsh",
            "swsh-fixo-giratina-max-lair: Swsh > Sv > Swsh",
            "swsh-forma-no-jogo-487-1: Swsh > Bdsp > Swsh",
            "swsh-forma-no-jogo-487-1: Swsh > Pla > Swsh",
            "swsh-forma-no-jogo-487-1: Swsh > Sv > Swsh",
            "swsh-forma-no-jogo-646-1: Swsh > Sv > Swsh",
            "swsh-forma-no-jogo-800-1: Swsh > Sv > Swsh",
        ];
        var found = new List<string>();
        foreach (var caso in Casos)
        {
            if (await fixture.SourceAsync(caso) is not { } source)
                continue;
            var (_, refusals, _) = Run(caso, source);
            found.AddRange(refusals.Where(r => r.Code == HomeTransfer.IllegalAfterMove).Select(r => r.Route));
        }
        Assert.Equal(expected, found.Order(StringComparer.Ordinal));
    }

    /// <summary>
    /// O que se perde está na conversão do PKHeX, antes de qualquer ajuste nosso: a volta crua é ilegal, e com o dado
    /// de volta (aqui, o nível Dynamax do original) seria válida.
    /// </summary>
    [Fact]
    public async Task Max_lair_que_saiu_do_sword_nao_volta_sem_o_nivel_dynamax()
    {
        var source = (PK8)await SourceAsync("swsh-fixo-giratina-max-lair");
        Assert.True(source.DynamaxLevel > 0);
        var inScarlet = Move(source, GameFamily.Sv);
        Assert.True(inScarlet.Ok, inScarlet.Refusal);

        var raw = (PK8)EntityConverter.ConvertToType(inScarlet.Pokemon!.Clone(), typeof(PK8), out _)!;
        Assert.Equal(0, raw.DynamaxLevel);
        Assert.False(new LegalityAnalysis(raw).Valid);
        raw.DynamaxLevel = source.DynamaxLevel;
        Assert.True(new LegalityAnalysis(raw).Valid);

        var back = Move(inScarlet.Pokemon, GameFamily.Swsh);
        Assert.False(back.Ok);
        Assert.Null(back.Pokemon);
        Assert.Equal(HomeTransfer.IllegalAfterMove, back.Refusal);
        Assert.Equal(HomeTransfer.IllegalAfterMove, HomeTransfer.Destinations(inScarlet.Pokemon, GameFamily.Sv)[GameFamily.Swsh]);
        // Para os outros jogos ele segue viagem.
        Assert.True(Move(inScarlet.Pokemon, GameFamily.Bdsp).Ok);
    }

    /// <summary>
    /// Defeito achado pela matriz: nem todo nascido no Z-A quebra a análise do PKHeX no Sword/Shield (o Pikachu quebra,
    /// e era recusado; o Aegislash na forma Blade não quebra, e saía ilegal). Agora a recusa é a mesma:
    /// "origin_not_accepted".
    /// </summary>
    [Fact]
    public async Task Nascido_no_za_que_o_pkhex_analisa_tambem_e_recusado_pela_origem()
    {
        var source = await SourceAsync("za-forma-no-jogo-681-1");
        Assert.Equal(GameVersion.ZA, source.Version);
        var raw = EntityConverter.ConvertToType(source.Clone(), typeof(PK8), out _);
        Assert.NotNull(raw);
        var la = new LegalityAnalysis(raw);
        Assert.True(la.Parsed);
        Assert.False(la.Valid);

        var moved = Move(source, GameFamily.Swsh);
        Assert.False(moved.Ok);
        Assert.Null(moved.Pokemon);
        Assert.Equal(HomeTransfer.OriginNotAccepted, moved.Refusal);
        Assert.Equal(HomeTransfer.OriginNotAccepted, HomeTransfer.Destinations(source, GameFamily.Za)[GameFamily.Swsh]);
    }

    /// <summary>A legalidade avisa, não bloqueia (D22): quem já era inválido continua sendo movido, e continua inválido.</summary>
    [Fact]
    public async Task Quem_ja_era_invalido_continua_sendo_movido()
    {
        var source = (await SourceAsync("swsh-eevee-nivel-100")).Clone();
        source.MetLevel = 100;
        source.CurrentLevel = 5; // nível abaixo do nível do encontro
        source.RefreshChecksum();
        Assert.Equal(LegalityStatus.Illegal, LegalityReporter.Analyze(source, "en").Status);

        var moved = Move(source, GameFamily.Sv);
        Assert.True(moved.Ok, moved.Refusal);
        Assert.IsType<PK9>(moved.Pokemon);
        Assert.Equal(LegalityStatus.Illegal, LegalityReporter.Analyze(moved.Pokemon!, "en").Status);
        Assert.Null(HomeTransfer.Destinations(source, GameFamily.Swsh)[GameFamily.Sv]);
    }
}
