using System.Security.Cryptography;
using PKHeX.Core;
using TradeRock.Core.Games;

namespace TradeRock.Core.Pokemon;

/// <summary>Resumo de um Pokémon de qualquer formato do cofre para listas e cards.</summary>
public sealed record PokemonSummary(
    StorageFormat Format,
    ushort Species,
    byte Form,
    string SpeciesName,
    string Nickname,
    byte Level,
    bool IsShiny,
    bool IsEgg,
    byte Gender,
    string OtName,
    ushort Tid,
    ushort Sid,
    string OriginGame,
    int Language,
    ushort HeldItem,
    bool Fateful,
    uint Pid)
{
    /// <summary>Jogo do catálogo de troca do formato, ou null (Gen 1, 2 e 4 a 7: não troca com o Switch).</summary>
    public GameFamily? Family => VaultFormats.Get(Format).Family;
}

/// <summary>Pokémon lido e normalizado: bytes decriptados, hash, resumo e o formato.</summary>
public sealed record PokemonFile(byte[] Data, string Sha256, PokemonSummary Summary)
{
    public VaultFormat Format => VaultFormats.Get(Summary.Format);
}

/// <summary>
/// Lê um Pokémon de qualquer formato do cofre (<see cref="VaultFormats"/>, Gen 1 a 9), decriptado ou criptografado.
/// - Gen 3: igual ao <see cref="Pk3Reader"/> (80 ou 100 bytes, hash dos bytes decriptados como vieram).
/// - Outros: guarda a forma "de caixa" decriptada (SIZE_STORED), para o mesmo Pokémon vindo de um arquivo de
///   equipe ou de caixa dar o mesmo hash. A forma de equipe é refeita pelo PKHeX quando precisa.
/// Alguns formatos têm o mesmo tamanho de arquivo (PK6/PK7/PB7, PK8/PB8, PK9/PA9; PK4/PK5 o PKHeX separa pelo
/// conteúdo): nesses casos o formato vem da extensão do arquivo ou de quem chamou (Pokémon recebido, save).
/// </summary>
public static class PokemonReader
{
    public const int MaxFileSize = 1024;

    /// <summary>Lê com o jogo do catálogo de troca (troca, construtor) ou sem nenhuma dica.</summary>
    public static PokemonFile Read(ReadOnlySpan<byte> file, GameFamily? family = null, string? fileName = null) =>
        ReadAs(file, family is { } f ? VaultFormats.FormatOf(f) : null, fileName);

    public static PokemonFile Read(ReadOnlySpan<byte> file, StorageFormat format) => ReadAs(file, format, null);

    private static PokemonFile ReadAs(ReadOnlySpan<byte> file, StorageFormat? format, string? fileName)
    {
        if (file.Length > MaxFileSize)
            throw new InvalidPokemonFileException("size", $"Arquivo com {file.Length} bytes: grande demais para um Pokémon.");

        var hinted = format ?? (fileName is null ? null : VaultFormats.ByExtension(Path.GetExtension(fileName))?.Format)
            ?? (Path.GetExtension(fileName ?? "") is ".ek3" ? StorageFormat.Pk3 : null);

        if (hinted is StorageFormat.Pk3 || (hinted is null && file.Length is Pk3Reader.StoredSize or Pk3Reader.PartySize))
            return FromPk3(Pk3Reader.Read(file));

        var pk = Parse(file, hinted);
        var found = VaultFormats.ByContext(pk.Context) is { } f && f.EntityType == pk.GetType() ? f
            : throw new InvalidPokemonFileException("game_unsupported",
                $"Pokémon de um formato que o RockAssistent não guarda ({pk.GetType().Name}).");
        if (hinted is { } h && h != found.Format)
            throw new InvalidPokemonFileException("game_mismatch", $"O arquivo é de {found.Name}, não de {VaultFormats.Get(h).Name}.");
        if (!pk.ChecksumValid)
            throw new InvalidPokemonFileException("checksum", "O checksum do Pokémon não confere (arquivo corrompido?).");
        if (pk.Species is 0 || pk.Species > pk.MaxSpeciesID)
            throw new InvalidPokemonFileException("species", $"Espécie inválida ({pk.Species}).");

        var data = new byte[pk.SIZE_STORED];
        pk.WriteDecryptedDataStored(data);
        return new PokemonFile(data, Convert.ToHexStringLower(SHA256.HashData(data)), Summarize(pk, found.Format));
    }

    /// <summary>Monta o PKM do PKHeX a partir dos bytes guardados (decriptados) de um formato conhecido.</summary>
    public static PKM ToEntity(byte[] data, StorageFormat format) =>
        format == StorageFormat.Pk3 ? new PK3(data.ToArray()) : Parse(data, format);

    public static PKM ToEntity(byte[] data, GameFamily family) => ToEntity(data, VaultFormats.FormatOf(family));

    public static PokemonSummary Summarize(PKM pk, GameFamily family) => Summarize(pk, VaultFormats.FormatOf(family));

    public static PokemonSummary Summarize(PKM pk, StorageFormat format)
    {
        // O PKHeX não tem nomes em português; os nomes em inglês são os mesmos usados no Brasil.
        var species = GameInfo.GetStrings("en").Species;
        var speciesName = pk.Species < species.Count ? species[pk.Species] : $"#{pk.Species}";
        return new PokemonSummary(
            Format: format,
            Species: pk.Species,
            Form: pk.Form,
            SpeciesName: speciesName,
            Nickname: pk.Nickname,
            Level: pk.CurrentLevel,
            IsShiny: pk.IsShiny,
            IsEgg: pk.IsEgg,
            Gender: pk.Gender,
            OtName: pk.OriginalTrainerName,
            Tid: pk.TID16,
            Sid: pk.SID16,
            OriginGame: pk.Version.ToString(),
            Language: pk.Language,
            HeldItem: (ushort)pk.HeldItem,
            Fateful: pk.FatefulEncounter,
            Pid: pk.PID);
    }

    private static PokemonFile FromPk3(Pk3File pk3)
    {
        var pk = new PK3(pk3.Data.ToArray());
        return new PokemonFile(pk3.Data, pk3.Sha256, Summarize(pk, StorageFormat.Pk3));
    }

    private static PKM Parse(ReadOnlySpan<byte> file, StorageFormat? format)
    {
        // Com o formato conhecido, monta direto o tipo do PKHeX: o EntityFormat confunde PB7 com PK7 e PA9 com PK9.
        if (format is { } known)
        {
            var info = VaultFormats.Get(known);
            if (info.Generation <= 2)
            {
                // Gen 1/2: o tamanho muda entre o japonês e o internacional; o PKHeX escolhe pelo tamanho.
                var gb = EntityFormat.GetFromBytes(file.ToArray(), info.Context);
                return gb is not null && gb.GetType() == info.EntityType ? gb
                    : throw new InvalidPokemonFileException("game_mismatch", $"Arquivo com {file.Length} bytes não é um Pokémon de {info.Name}.");
            }
            var blank = Blank(info.EntityType);
            if (file.Length != blank.SIZE_STORED && file.Length != blank.SIZE_PARTY)
                throw new InvalidPokemonFileException("game_mismatch", $"Arquivo com {file.Length} bytes não é um Pokémon de {info.Name}.");
            return Create(known, file.ToArray());
        }

        return EntityFormat.GetFormat(file) switch
        {
            EntityFormatDetected.None => throw new InvalidPokemonFileException("size",
                $"Arquivo com {file.Length} bytes não é um Pokémon que o PKHeX reconheça."),
            EntityFormatDetected.FormatPK8 => new PK8(file.ToArray()),
            EntityFormatDetected.FormatPB8 => new PB8(file.ToArray()),
            EntityFormatDetected.FormatPA8 => new PA8(file.ToArray()),
            EntityFormatDetected.FormatPA9 => new PA9(file.ToArray()),
            EntityFormatDetected.FormatPB7 => new PB7(file.ToArray()),
            // Mesmo tamanho em dois jogos: PK8/PB8, PK9/PA9 (que o PKHeX também chama de PK9) e PK6/PK7/PB7.
            EntityFormatDetected.Format8or8b or EntityFormatDetected.Format9or9a or EntityFormatDetected.FormatPK9
                or EntityFormatDetected.Format6or7 or EntityFormatDetected.FormatPK7 => throw new InvalidPokemonFileException(
                "format_ambiguous",
                "Não dá para saber o jogo só pelo conteúdo: use a extensão do arquivo (.pk6, .pk7, .pb7, .pk8, .pb8, .pk9 ou .pa9)."),
            var other => EntityFormat.GetFromBytes(file.ToArray(), EntityContext.None)
                ?? throw new InvalidPokemonFileException("size", $"O PKHeX não reconheceu o arquivo como Pokémon ({other})."),
        };
    }

    private static PKM Create(StorageFormat format, byte[] data) => format switch
    {
        StorageFormat.Pk3 => new PK3(data),
        StorageFormat.Pk4 => new PK4(data),
        StorageFormat.Pk5 => new PK5(data),
        StorageFormat.Pk6 => new PK6(data),
        StorageFormat.Pk7 => new PK7(data),
        StorageFormat.Pb7 => new PB7(data),
        StorageFormat.Pk8 => new PK8(data),
        StorageFormat.Pb8 => new PB8(data),
        StorageFormat.Pa8 => new PA8(data),
        StorageFormat.Pk9 => new PK9(data),
        StorageFormat.Pa9 => new PA9(data),
        _ => throw new ArgumentOutOfRangeException(nameof(format)),
    };

    private static readonly Dictionary<Type, PKM> Blanks = [];

    private static PKM Blank(Type type)
    {
        lock (Blanks)
        {
            if (!Blanks.TryGetValue(type, out var blank))
                Blanks[type] = blank = (PKM)Activator.CreateInstance(type)!;
            return blank;
        }
    }
}
