using TradeRock.Core.Games;
using TradeRock.Core.Trading;

namespace TradeRock.Core.Storage;

public enum PokemonOrigin
{
    Imported,
    Received,
    Demo,
    Created,
}

/// <summary>
/// Pokémon guardado. O arquivo decriptado (.pk3, .pk8, .pk9...) fica em disco, nomeado pelo SHA-256;
/// aqui ficam o hash e o resumo para listar e buscar. A Entrega 4 (cofre) amplia esta tabela.
/// </summary>
public sealed class StoredPokemon : IOwned
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public required string Sha256 { get; set; }
    public int Size { get; set; }

    /// <summary>Formato do arquivo (D34; de pk1 a pa9). Antes era o jogo do catálogo de troca (coluna Family).</summary>
    public StorageFormat Format { get; set; }

    /// <summary>Jogo do catálogo de troca do formato, ou null: Gen 1, 2 e 4 a 7 ficam no cofre, mas não trocam.</summary>
    public GameFamily? Family => VaultFormats.Get(Format).Family;

    public ushort Species { get; set; }

    /// <summary>Forma da espécie (Alola, Galar, Hisui...); 0 na Gen 3, menos o Unown.</summary>
    public byte Form { get; set; }

    public required string SpeciesName { get; set; }
    public required string Nickname { get; set; }
    public byte Level { get; set; }
    public bool IsShiny { get; set; }
    public bool IsEgg { get; set; }
    public byte Gender { get; set; }
    public required string OtName { get; set; }
    public ushort Tid { get; set; }
    public ushort Sid { get; set; }
    public required string OriginGame { get; set; }

    /// <summary>Item segurado (ID interno do jogo do formato).</summary>
    public ushort HeldItem { get; set; }

    /// <summary>Flag de encontro fatídico (eventos; na Gen 3, Mew/Deoxys/Lugia/Ho-Oh dos tickets).</summary>
    public bool Fateful { get; set; }

    /// <summary>Resultado do LegalityAnalysis ("legal", "illegal", "unverifiable"); null = ainda não analisado.</summary>
    public string? Legality { get; set; }

    /// <summary>Versão do PKHeX.Core que fez a análise (refeita quando o PKHeX é atualizado).</summary>
    public string? LegalityPkhexVersion { get; set; }

    public PokemonOrigin Origin { get; set; }
    public string? SourceFileName { get; set; }
    public DateTimeOffset AddedAt { get; set; }

    /// <summary>PID (para achar "possíveis duplicatas": mesmo Pokémon com dados diferentes). Null = ainda não lido.</summary>
    public uint? Pid { get; set; }

    /// <summary>
    /// SHA-256 do arquivo de antes da ida ao Sword/Shield (D37), guardado no mesmo lugar dos outros arquivos do cofre.
    /// Só existe enquanto o Pokémon está no Sword/Shield e a ida perdeu a origem; null nos outros casos.
    /// </summary>
    public string? OriginalSha256 { get; set; }

    /// <summary>Formato do arquivo de <see cref="OriginalSha256"/>.</summary>
    public StorageFormat? OriginalFormat { get; set; }

    /// <summary>Caixa do cofre; null = Caixa de entrada.</summary>
    public Guid? BoxId { get; set; }

    /// <summary>Posição na caixa (0–29); null na Caixa de entrada.</summary>
    public int? Slot { get; set; }

    /// <summary>Na lixeira desde; null = ativo.</summary>
    public DateTimeOffset? DeletedAt { get; set; }

    public List<PokemonTag> Tags { get; set; } = [];
}

/// <summary>Caixa do cofre (30 posições, como no jogo).</summary>
public sealed class VaultBox : IOwned
{
    public const int Slots = 30;

    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public required string Name { get; set; }
    public int Order { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public sealed class Tag : IOwned
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public required string Name { get; set; }

    /// <summary>Nome de cor da paleta da interface (ex.: "sky", "amber").</summary>
    public required string Color { get; set; }
}

public sealed class PokemonTag
{
    public Guid PokemonId { get; set; }
    public Guid TagId { get; set; }
}

/// <summary>Histórico de uma sessão de troca (uma ou mais trocas com o mesmo Switch).</summary>
public sealed class TradeRecord : IOwned
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }
    public required string EngineId { get; set; }
    public bool IsSimulation { get; set; }
    public TradeGame Game { get; set; }
    public required string TrainerName { get; set; }
    public ushort Tid { get; set; }
    public ushort? Sid { get; set; }
    public TradeSessionStatus Status { get; set; }
    public int Attempts { get; set; }
    public int TradesPlanned { get; set; }
    public int TradesCompleted { get; set; }
    public string? FailureCode { get; set; }
    public string? FailureMessage { get; set; }

    /// <summary>Eventos da sessão em JSON (sem dados de chave: os motores nunca os emitem).</summary>
    public required string EventsJson { get; set; }

    public List<TradeRecordEntry> Entries { get; set; } = [];
}

public sealed class TradeRecordEntry
{
    public int Id { get; set; }
    public Guid TradeRecordId { get; set; }
    public int TradeIndex { get; set; }
    public Guid SentPokemonId { get; set; }
    public Guid? ReceivedPokemonId { get; set; }
}

/// <summary>Treinador simulado salvo (preset).</summary>
public sealed class TrainerPreset : IOwned
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public required string Label { get; set; }
    public required string Name { get; set; }
    public ushort Tid { get; set; }
    public ushort? Sid { get; set; }
    public TradeGame Game { get; set; }
}

/// <summary>
/// Pokémon do cofre "anexado" a uma posição de um save do PC: quando o slot muda no jogo (subiu de nível,
/// evoluiu), a sincronização (<c>POST /api/boxes/sync-attachments</c>) substitui os dados do cofre.
/// </summary>
public sealed class SaveAttachment : IOwned
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public Guid PokemonId { get; set; }
    public required string SavePath { get; set; }

    /// <summary>SHA-256 do save na última vez que o anexo foi conferido (igual = nada mudou).</summary>
    public required string SaveSha256 { get; set; }

    /// <summary>Caixa (a partir de 0) ou -1 para o time.</summary>
    public int Box { get; set; }

    public int Slot { get; set; }
    public DateTimeOffset AttachedAt { get; set; }
}

/// <summary>Hunt de shiny (Contador Hunt): espécie, jogo, método, modificadores, contador e cronômetro.</summary>
public sealed class ShinyHunt : IOwned
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public int Species { get; set; }
    public int Form { get; set; }

    /// <summary>Chave do jogo (<c>GameVersions</c>: sv, bdsp, frlg...).</summary>
    public required string GameKey { get; set; }

    /// <summary>Id do método em <c>shiny-methods.json</c>.</summary>
    public required string MethodId { get; set; }

    /// <summary>Modificadores em JSON: id → "true"/"false" (toggle) ou o número em texto (steps).</summary>
    public required string ModifiersJson { get; set; }

    public int Count { get; set; }
    public long ElapsedSeconds { get; set; }
    public DateTimeOffset StartedAt { get; set; }

    /// <summary>Quando terminou (achou ou desistiu); null = em andamento.</summary>
    public DateTimeOffset? EndedAt { get; set; }

    public bool Found { get; set; }
    public string? Notes { get; set; }
}

/// <summary>
/// Registro de uma atividade terminada (raid, Mystery Gift ou Union Circle) para o Histórico. Gravado pelo
/// <c>ActivityService</c> no fim da sessão (sucesso, cancelamento ou erro). Nunca guarda o arquivo do presente.
/// </summary>
public sealed class ActivityRecord : IOwned
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }

    /// <summary><c>raid</c>, <c>gift</c> ou <c>circle</c>.</summary>
    public required string Kind { get; set; }

    /// <summary>Tipo da raid (<c>max-raid-host</c>...) ou do Union Circle (<c>quests</c>, <c>encounters</c>).</summary>
    public string? SubKind { get; set; }

    public TradeGame Game { get; set; }
    public required string EngineId { get; set; }
    public bool IsSimulation { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset EndedAt { get; set; }

    /// <summary><c>completed</c>, <c>failed</c> ou <c>cancelled</c>.</summary>
    public required string Status { get; set; }

    public bool Ok { get; set; }
    public string? ErrorCode { get; set; }

    /// <summary>Nome do presente ou título do evento da raid.</summary>
    public string? Label { get; set; }

    /// <summary>Espécie do encontro do Union Circle ou o lendário da Dynamax Adventure.</summary>
    public int? Species { get; set; }

    /// <summary>Resumo em JSON (raid: evento, chefe, jogadores, Gigantamax; circle: missão, BP, nível, shiny, tera; gift: modo, chave, extensão).</summary>
    public required string SummaryJson { get; set; }
}

/// <summary>
/// Run de Nuzlocke: jogo (e versão, se a pessoa escolheu), regras, estado e a área em que está. Os Pokémon
/// (<see cref="NuzlockeMon"/>) e os eventos (<see cref="NuzlockeEvent"/>) pertencem à run e só se chega a eles por ela.
/// </summary>
public sealed class NuzlockeRun : IOwned
{
    public Guid Id { get; set; }
    public Guid OwnerId { get; set; }
    public required string Name { get; set; }

    /// <summary>Chave do jogo (<c>GameVersions</c>).</summary>
    public required string GameKey { get; set; }

    /// <summary>Versão do PokeAPI escolhida (<c>firered</c>, <c>scarlet</c>...); null = todas as do jogo.</summary>
    public string? Version { get; set; }

    /// <summary>
    /// Inicial escolhido pelo jogador (identificador do PokeAPI, <c>GameStarters</c>); null = não informado. Escolhe o
    /// time dos chefes que dependem dele (<c>BossBattle.Variants</c>).
    /// </summary>
    public string? Starter { get; set; }

    /// <summary><c>NuzlockeRules</c> em JSON.</summary>
    public required string RulesJson { get; set; }

    /// <summary><c>active</c>, <c>paused</c>, <c>champion</c> ou <c>wipe</c>.</summary>
    public required string Status { get; set; }

    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset? EndedAt { get; set; }

    /// <summary>Id da área em <c>assets/data/areas</c>; null = a primeira do jogo.</summary>
    public string? CurrentAreaId { get; set; }
}

/// <summary>Pokémon de uma run (o encontro registrado e o que aconteceu com ele).</summary>
public sealed class NuzlockeMon
{
    public Guid Id { get; set; }
    public Guid RunId { get; set; }
    public required string AreaId { get; set; }

    /// <summary>Espécie; 0 = sem encontro (só com <c>missed</c> ou <c>failed</c>).</summary>
    public int Species { get; set; }

    public int Form { get; set; }
    public string? Nickname { get; set; }
    public int Level { get; set; }

    /// <summary><c>party</c>, <c>box</c>, <c>dead</c>, <c>missed</c> ou <c>failed</c>.</summary>
    public required string Status { get; set; }

    public bool Shiny { get; set; }
    public string? DeathCause { get; set; }
    public DateTimeOffset CaughtAt { get; set; }
    public DateTimeOffset? DiedAt { get; set; }

    /// <summary>
    /// Inicial que a run pôs na equipe sozinha. Só conta como o encontro da área quando o jogo o dá numa cidade
    /// (<c>kind: city</c>); dado numa rota ou lago (Route 101, Lake Verity), o primeiro selvagem ainda vale.
    /// </summary>
    public bool IsStarter { get; set; }

    /// <summary>Golpes atuais em JSON: lista de identificadores do PokeAPI (<c>["thunder-shock"]</c>).</summary>
    public required string MovesJson { get; set; }
}

/// <summary>Evento de uma run (para o histórico e as estatísticas): tipo e os dados em JSON.</summary>
public sealed class NuzlockeEvent
{
    public Guid Id { get; set; }
    public Guid RunId { get; set; }
    public DateTimeOffset At { get; set; }

    /// <summary><c>run_started</c>, <c>area_changed</c>, <c>encounter</c>, <c>mon_updated</c>, <c>boss_defeated</c> ou <c>run_finished</c>.</summary>
    public required string Kind { get; set; }

    public required string Json { get; set; }
}

/// <summary>Configuração chave → JSON.</summary>
public sealed class SettingEntry
{
    public required string Key { get; set; }
    public required string Json { get; set; }
}

public enum UserRole
{
    User,
    Admin,
}

/// <summary>
/// Conta do modo servidor. A conta com Id = <see cref="OwnerScope.LocalOwner"/> é o dono do PC (sempre
/// administrador); ela só entra pela rede depois de ganhar nome e senha.
/// </summary>
public sealed class UserAccount
{
    public Guid Id { get; set; }

    /// <summary>Nome de login, em minúsculas. Null só na conta do dono ainda sem acesso pela rede.</summary>
    public string? Username { get; set; }

    /// <summary>Argon2id no formato PHC ($argon2id$v=19$m=...,t=...,p=...$sal$hash). Null = não entra pela rede.</summary>
    public string? PasswordHash { get; set; }

    public UserRole Role { get; set; }
    public bool Disabled { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastLoginAt { get; set; }
}
