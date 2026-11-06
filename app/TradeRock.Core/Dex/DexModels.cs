namespace TradeRock.Core.Dex;

/// <summary>Filtro da lista da Pokédex. <see cref="Type"/> é o identificador do PokeAPI ("ghost").</summary>
public sealed record DexQuery(string? Text, string? Type, int? Generation, string? GameKey, int Offset = 0, int Limit = 100);

/// <summary>Uma linha da lista. Nome em inglês; os tipos são os da forma padrão hoje.</summary>
public sealed record SpeciesSummary(int Id, string Name, int Generation, string[] Types, int? NationalNumber);

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
public sealed record SpeciesForm(int Id, string Identifier, string Name, string[] Types, BaseStats Stats, bool IsMega, bool IsDefault);

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

/// <summary>Como conseguir a espécie no jogo. <see cref="Gift"/> vem dos nossos dados (Task 8).</summary>
public enum AvailabilityKind
{
    None,
    Encounter,
    Evolution,
    Egg,
    Gift,
}

public sealed record GameAvailability(string GameKey, string Name, AvailabilityKind Kind);
