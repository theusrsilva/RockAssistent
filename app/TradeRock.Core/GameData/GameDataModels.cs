namespace TradeRock.Core.GameData;

// Dados dos jogos embutidos em assets/data (JSON em snake_case). Cada arquivo tem "source" (URLs e decompilações
// consultadas) e "items"; os mapas têm "region", "nodes" e "edges" no lugar de "items".

/// <summary>Um método de caça de shiny. As odds são o denominador: 4096 quer dizer 1/4096.</summary>
/// <remarks>
/// <see cref="ShinyModifier.Odds"/> e <see cref="ShinyStep.Odds"/> são as odds do método com só aquele modificador
/// (ou só aquele degrau). Para juntar vários, converta cada odd em "rolagens" de 1/base e some os acréscimos.
/// </remarks>
public sealed record ShinyMethod(
    string Id,
    string Name,
    string Description,
    double BaseOdds,
    IReadOnlyList<ShinyModifier> Modifiers,
    IReadOnlyList<string> Games,
    string? Notes,
    bool Verified);

/// <summary>Modificador: <c>toggle</c> (liga/desliga, com <see cref="Odds"/>) ou <c>steps</c> (degraus).</summary>
public sealed record ShinyModifier(string Id, string Name, string Kind, IReadOnlyList<ShinyStep>? Steps, double? Odds);

/// <summary>Um degrau de um modificador (contagem de <see cref="Min"/> até <see cref="Max"/>; sem máximo = em diante).</summary>
public sealed record ShinyStep(string Label, int Min, int? Max, double Odds);

/// <summary>Batalha importante (líder, rival, equipe do mal, Elite 4, campeão).</summary>
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
    string? Notes);

/// <summary>Pokémon de um time. Golpes com os identificadores do PokeAPI.</summary>
public sealed record BossPokemon(
    int Species,
    int Form,
    int Level,
    IReadOnlyList<string> Moves,
    string? Ability,
    string? Item,
    string? TeraType);

public sealed record TrainerBattle(
    string Id,
    string Area,
    string Class,
    string Name,
    IReadOnlyList<BossPokemon> Party,
    bool Verified);

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
