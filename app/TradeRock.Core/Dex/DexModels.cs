namespace TradeRock.Core.Dex;

/// <summary>Filtro da lista da Pokédex. <see cref="Type"/> é o identificador do PokeAPI ("ghost").</summary>
/// <param name="RegionalForms">
/// Lista também as formas regionais (Alola, Galar, Hisui, Paldea), cada uma numa linha logo depois da espécie. Só elas:
/// mudam o tipo do Pokémon. As outras formas (Mega, Gigantamax...) ficam na página da espécie.
/// </param>
/// <param name="OtherForms">
/// Lista também as outras formas que se guardam como estão (Rotom, Deoxys, Lycanroc, Oricorio...). Ficam de fora Mega,
/// Gigantamax, Totem, as que só existem em batalha, as fusões e as que dependem de item.
/// </param>
public sealed record DexQuery(string? Text, string? Type, int? Generation, string? GameKey, int Offset = 0, int Limit = 100,
    bool RegionalForms = false, bool OtherForms = false);

/// <summary>Uma linha da lista. Nome em inglês; os tipos são os da forma padrão hoje.</summary>
/// <param name="FormId">Id do <c>pokemon</c> do PokeAPI quando a linha é uma forma regional (sprite e página); null na espécie.</param>
/// <param name="Region">Região da forma: <c>alola</c>, <c>galar</c>, <c>hisui</c> ou <c>paldea</c>.</param>
/// <param name="FormDetail">
/// Complemento do nome, em inglês: nas regionais, o que vem depois da região (as três raças do Tauros de Paldea); nas
/// outras formas, o nome da forma ("Wash Rotom", "Midnight Form").
/// </param>
public sealed record SpeciesSummary(int Id, string Name, int Generation, string[] Types, int? NationalNumber,
    int? FormId = null, string? Region = null, string? FormDetail = null);

/// <summary>Ficha de uma espécie vista num jogo (grupo de versões).</summary>
public sealed record SpeciesDetail(
    int Id,
    string Name,
    IReadOnlyDictionary<string, string> Names,
    string? Genus,
    string[] Types,
    int Height,
    int Weight,
    int GenderRate,
    int CaptureRate,
    int? HatchSteps,
    int? BaseHappiness,
    string? GrowthRate,
    string? Color,
    string[] EggGroups,
    IReadOnlyList<SpeciesAbility> Abilities,
    IReadOnlyList<EvolutionStep> EvolutionChain,
    IReadOnlyDictionary<string, double> DamageTaken,
    IReadOnlyList<FlavorText> FlavorTexts,
    IReadOnlyList<EncounterInfo> Encounters,
    SpeciesMoves Moves,
    BaseStats Stats,
    IReadOnlyList<SpeciesForm> Forms,
    IReadOnlyDictionary<string, int> DexNumbers,
    bool ExistsInGame);

public sealed record SpeciesAbility(int Id, string Name, bool IsHidden, string? Effect);

/// <summary>
/// Uma etapa da cadeia de evolução. <see cref="Conditions"/> tem uma frase curta (em inglês, como os nomes de golpes
/// e itens) por jeito de evoluir, como "level 25" ou "trade holding Metal Coat".
/// </summary>
public sealed record EvolutionStep(int SpeciesId, string Name, int? FromSpeciesId, IReadOnlyList<string> Conditions);

public sealed record FlavorText(string Version, string Language, string Text);

/// <summary>Encontros juntados por versão, área, método e condições (níveis de/até, chance somada).</summary>
public sealed record EncounterInfo(string Version, string Location, string? Area, string Method, int MinLevel,
    int MaxLevel, int Chance, string[] Conditions);

/// <summary>
/// Encontro de um local do PokeAPI (todas as sub-áreas juntas) numa versão, por espécie, forma e método. <see cref="Form"/>
/// é a posição da forma na ficha (0 = a padrão), como em <see cref="SpeciesDetail.Forms"/>.
/// </summary>
public sealed record LocationEncounter(int Species, int Form, string Version, string Method, int MinLevel, int MaxLevel, int Chance);

/// <summary>
/// TM/HM/TR do jogo (tabela <c>machines</c> do PokeAPI, grupo de versões principal). <see cref="Machine"/> é o nome do
/// item ("TM30", "HM03", "TR12"); o tipo e os números do golpe são os do jogo. <see cref="Dlc"/>: só existe na DLC.
/// </summary>
public sealed record MachineInfo(int Number, string Machine, int MoveId, string MoveIdentifier, string MoveName, string Type,
    string? DamageClass, int? Power, int? Accuracy, int? Pp, bool Dlc);

public sealed record SpeciesMoves(IReadOnlyList<LearnedMove> LevelUp, IReadOnlyList<LearnedMove> Machine,
    IReadOnlyList<LearnedMove> Tutor, IReadOnlyList<LearnedMove> Egg)
{
    public static SpeciesMoves Empty { get; } = new([], [], [], []);
}

/// <summary>
/// Golpe aprendido, com tipo e força da geração do jogo. <see cref="Identifier"/>: o do PokeAPI (<c>thunder-punch</c>, como
/// nos golpes dos chefes). <see cref="Machine"/> é o nome do TM/HM ("TM30").
/// </summary>
public sealed record LearnedMove(int Id, string Identifier, string Name, string Type, string? Class, int? Power, int? Accuracy, int? Pp,
    int? Level, int? MachineNumber, string? Machine);

/// <summary>Status base, total e os pontos de esforço (EVs) que a espécie dá ao ser derrotada.</summary>
public sealed record BaseStats(int Hp, int Attack, int Defense, int SpecialAttack, int SpecialDefense, int Speed, int Total,
    IReadOnlyDictionary<string, int> Evs)
{
    public static BaseStats Empty { get; } = new(0, 0, 0, 0, 0, 0, 0, new Dictionary<string, int>());
}

/// <summary>Uma forma (linha de <c>pokemon</c>): a padrão, regionais, Mega, Gigantamax...</summary>
/// <param name="Listed">A forma tem cartão próprio na lista da Pokédex (regional ou "outra forma"): dá para marcar na coleção.</param>
public sealed record SpeciesForm(int Id, string Identifier, string Name, string[] Types, BaseStats Stats, bool IsMega, bool IsDefault,
    bool Listed = false);

public sealed record MoveDetail(int Id, string Identifier, string Name, string Type, string? Class, int? Power, int? Accuracy,
    int? Pp, int Priority, int? EffectChance, string? Effect, int Generation, bool ExistsInGame, int? MachineNumber,
    string? Machine, IReadOnlyList<MoveLearner> Learners);

/// <summary>Quem aprende o golpe no jogo e como.</summary>
public sealed record MoveLearner(int SpeciesId, string Name, string Method, int? Level);

public sealed record AbilityDetail(int Id, string Identifier, string Name, string? ShortEffect, string? Effect, int Generation,
    IReadOnlyList<AbilityHolder> Pokemon);

public sealed record AbilityHolder(int SpeciesId, int PokemonId, string Name, bool IsHidden);

public sealed record ItemDetail(int Id, string Identifier, string Name, string? Category, int? Cost, string? Effect, int? FlingPower);

/// <summary><c>Factors[ataque][defesa]</c>: 0, 0.5, 1 ou 2.</summary>
public sealed record TypeChart(int Generation, IReadOnlyList<string> Types,
    IReadOnlyDictionary<string, IReadOnlyDictionary<string, double>> Factors);

/// <summary>Como conseguir a espécie no jogo. <see cref="Gift"/> e <see cref="Trade"/> (troca com personagem) vêm dos nossos dados.</summary>
public enum AvailabilityKind
{
    None,
    Encounter,
    Evolution,
    Egg,
    Gift,
    Trade,
}

/// <param name="ShinyLocked">Dá para conseguir, mas todo encontro é travado para shiny (tabelas do PKHeX) e não dá para chocar.</param>
/// <param name="Requires">
/// Nulo, ou o que todo jeito de conseguir a espécie no jogo precisa: <see cref="Dex.Requires.Pokewalker"/> ou
/// <see cref="Dex.Requires.EventItem"/>.
/// </param>
public sealed record GameAvailability(string GameKey, string Name, AvailabilityKind Kind, bool ShinyLocked = false,
    string? Requires = null);

/// <summary>Requisitos de <see cref="GameAvailability.Requires"/>.</summary>
public static class Requires
{
    /// <summary>Só pelo Pokéwalker (acessório do HeartGold/SoulSilver).</summary>
    public const string Pokewalker = "pokewalker";

    /// <summary>Só num lugar ou evento aberto por item de evento recebido por Mystery Gift.</summary>
    public const string EventItem = "event_item";
}

/// <summary>O essencial de um golpe para análise de time: tipo, categoria (physical/special/status) e poder.</summary>
public sealed record MoveBattleInfo(int Id, string Identifier, string Name, string Type, string? Class, int? Power);

/// <summary>Uma marcação da coleção: a espécie (<paramref name="FormId"/> 0) ou uma forma da lista, normal ou shiny.</summary>
public sealed record DexOwned(int SpeciesId, int FormId, bool Shiny);

/// <summary>
/// A coleção de uma visão ("" = geral; ou a chave de um jogo): o que a pessoa marcou e o que está no cofre agora.
/// </summary>
public sealed record DexCollection(string Scope, IReadOnlyList<DexOwned> Marks, IReadOnlyList<DexOwned> Vault);
