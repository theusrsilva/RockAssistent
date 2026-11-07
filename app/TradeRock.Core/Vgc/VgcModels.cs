namespace TradeRock.Core.Vgc;

// Dados do VGC embutidos em assets/data/vgc (JSON em snake_case, lidos pelo GameDataStore).

/// <summary>
/// Um time sugerido, escrito por nós (arquétipo comum da regulamentação, nunca o time de um jogador).
/// <see cref="Paste"/> está no formato do Showdown: 6 Pokémon com item, habilidade, EVs, natureza e 4 golpes.
/// </summary>
public sealed record VgcTeam(
    string Id,
    string Name,
    string Archetype,
    string Regulation,
    string DateIso,
    string Paste,
    string IdeaPt,
    string IdeaEn,
    string HowToPlayPt,
    string HowToPlayEn,
    string DangersPt,
    string DangersEn);

/// <summary>
/// A regulamentação do VGC vigente na data de <see cref="CheckedOn"/>. A lista completa de Pokémon permitidos fica no
/// jogo (o roster); aqui vão as regras, as datas, as adições da regulamentação e o que foi banido.
/// <see cref="SmogonFormat"/> é o nome do arquivo de estatísticas do Smogon (<c>chaos/&lt;formato&gt;.json</c>) que a
/// opção de internet baixa; sem ele vale <see cref="SmogonStats.DefaultFormat"/>.
/// </summary>
public sealed record VgcRegulation(
    IReadOnlyList<string> Source,
    string Name,
    string Game,
    string ValidFrom,
    string? ValidTo,
    string CheckedOn,
    string Format,
    int Bring,
    int Pick,
    int Level,
    IReadOnlyList<string> AllowedDexes,
    IReadOnlyList<string> BannedSpecies,
    IReadOnlyList<string> BannedMoves,
    IReadOnlyList<string> NewSpecies,
    IReadOnlyList<string> NewMegas,
    IReadOnlyList<string> NewItems,
    IReadOnlyList<string> RulesPt,
    IReadOnlyList<string> RulesEn,
    string NotesPt,
    string NotesEn,
    bool Verified,
    string? SmogonFormat = null);
