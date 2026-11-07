namespace TradeRock.Core.GameData;

// Dados dos jogos embutidos em assets/data (JSON em snake_case). Cada arquivo tem "source" (URLs e decompilações
// consultadas) e "items"; os mapas têm "region", "nodes" e "edges" no lugar de "items".

/// <summary>Um método de caça de shiny. As odds são o denominador: 4096 quer dizer 1/4096.</summary>
/// <remarks>
/// <para><c>Odds</c> de um modificador ou degrau é a odd do método só com ele (o valor para mostrar sozinho).</para>
/// <para>Para combinar modificadores, use as rolagens: com <see cref="BaseRolls"/> e <see cref="RollOdds"/>
/// preenchidos, a odd final é <c>1 / (1 - (1 - 1/RollOdds)^(BaseRolls + Σ Rolls))</c>, com a soma das
/// <c>Rolls</c> dos toggles ligados e do degrau escolhido de cada <c>steps</c> (<see cref="Hunts.ShinyOdds.FromRolls"/>).</para>
/// <para>Método com <see cref="BaseRolls"/> nulo não segue rolagens (Ovo Estranho, Dynamax Adventure, Ultra
/// Wormhole, Tera Raid, Poké Radar, DexNav...): mostre a <c>Odds</c> do modificador ou degrau como está e
/// <b>nunca combine</b>. Num método com rolagens, todo modificador e degrau tem <c>Rolls</c>.</para>
/// </remarks>
public sealed record ShinyMethod(
    string Id,
    string Name,
    string NameEn,
    string Description,
    double BaseOdds,
    int? BaseRolls,
    double? RollOdds,
    IReadOnlyList<ShinyModifier> Modifiers,
    IReadOnlyList<string> Games,
    string? Notes,
    bool Verified);

/// <summary>
/// Modificador: <c>toggle</c> (liga/desliga, com <see cref="Odds"/>) ou <c>steps</c> (degraus).
/// <see cref="Rolls"/>: rolagens a mais sobre a base (só toggle). <see cref="Counter"/>: o <c>steps</c> que segue o
/// contador da hunt (no máximo um por método). <see cref="Games"/>: jogos em que vale (nulo = todos os do método).
/// </summary>
public sealed record ShinyModifier(
    string Id,
    string Name,
    string NameEn,
    string Kind,
    IReadOnlyList<ShinyStep>? Steps,
    double? Odds,
    int? Rolls,
    bool Counter,
    IReadOnlyList<string>? Games);

/// <summary>
/// Um degrau de um modificador (contagem de <see cref="Min"/> até <see cref="Max"/>; sem máximo = em diante).
/// <see cref="Rolls"/>: rolagens a mais sobre a base.
/// </summary>
public sealed record ShinyStep(string Label, int Min, int? Max, double Odds, int? Rolls);

/// <summary>Batalha importante (líder, rival, equipe do mal, Elite 4, campeão).</summary>
/// <remarks><see cref="Versions"/>: versões do PokeAPI em que a batalha existe (ex.: <c>["ruby"]</c>); nulo = todas as do jogo.</remarks>
public sealed record BossBattle(
    string Id,
    string Kind,
    string Name,
    string? Title,
    string? Location,
    int Order,
    string? Badge,
    IReadOnlyList<BossPokemon> Party,
    int LevelCap,
    bool Verified,
    string? Notes,
    IReadOnlyList<string>? Versions);

/// <summary>Pokémon de um time. Golpes com os identificadores do PokeAPI.</summary>
public sealed record BossPokemon(
    int Species,
    int Form,
    int Level,
    IReadOnlyList<string> Moves,
    string? Ability,
    string? Item,
    string? TeraType);

/// <summary>Treinador comum (não chefe) de uma área.</summary>
/// <remarks><see cref="Versions"/>: versões do PokeAPI em que o treinador existe (ex.: Team Magma só no Ruby); nulo = todas.</remarks>
public sealed record TrainerBattle(
    string Id,
    string Area,
    string Class,
    string Name,
    IReadOnlyList<BossPokemon> Party,
    bool Verified,
    IReadOnlyList<string>? Versions);

/// <summary>Uma área do jogo, em ordem de progressão.</summary>
public sealed record GameArea(
    string Id,
    string Name,
    string Kind,
    int Order,
    string? MapNode,
    int? PokeApiLocationId,
    IReadOnlyList<string> BossesBefore);

/// <summary>Encontro escrito por nós (o PokeAPI não cobre tudo). <see cref="Alpha"/> só nos jogos Legends.</summary>
public sealed record OwnEncounter(
    string Area,
    int Species,
    int Form,
    string Method,
    int MinLevel,
    int MaxLevel,
    string? Chance,
    string? Conditions,
    IReadOnlyList<string> Versions,
    bool Alpha,
    bool Verified);

public sealed record MapGraph(string Region, IReadOnlyList<MapNode> Nodes, IReadOnlyList<MapEdge> Edges);

/// <summary>Nó do mapa. X e Y de 0 a 1. <see cref="Games"/> nulo = todos os jogos da região.</summary>
public sealed record MapNode(string Id, string Name, string Kind, double X, double Y, IReadOnlyList<string>? Games);

public sealed record MapEdge(string From, string To);

/// <summary>Formato de um arquivo de lista.</summary>
internal sealed record DataFile<T>(IReadOnlyList<string> Source, IReadOnlyList<T> Items);

/// <summary>Formato de um arquivo de mapa.</summary>
internal sealed record MapFile(IReadOnlyList<string> Source, string Region, IReadOnlyList<MapNode> Nodes, IReadOnlyList<MapEdge> Edges);
