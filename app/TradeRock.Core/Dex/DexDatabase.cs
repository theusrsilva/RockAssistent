using System.Security.Cryptography;
using Microsoft.Data.Sqlite;

namespace TradeRock.Core.Dex;

/// <summary>
/// A Pokédex offline (dex.sqlite do PokeAPI, embutido). Na primeira vez é copiada para
/// <c>&lt;dados&gt;/dex/&lt;sha8&gt;.sqlite</c> (os 8 primeiros hexadecimais do SHA-256); as cópias de versões
/// antigas são apagadas. Abre só para leitura.
/// </summary>
public sealed class DexDatabase
{
    public const string ResourceName = "data/dex.sqlite";

    private readonly string _folder;
    private readonly Lazy<string> _file;

    public DexDatabase(AppPaths paths)
    {
        _folder = Path.Combine(paths.DataDir, "dex");
        _file = new Lazy<string>(Prepare, LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public static bool ResourceExists =>
        typeof(DexDatabase).Assembly.GetManifestResourceInfo(ResourceName) is not null;

    /// <summary>Caminho da cópia em uso (faz a cópia se precisar).</summary>
    public string FilePath => _file.Value;

    /// <summary>Uma conexão aberta, só leitura. Quem chama fecha.</summary>
    public SqliteConnection Open()
    {
        var builder = new SqliteConnectionStringBuilder
        {
            DataSource = FilePath,
            Mode = SqliteOpenMode.ReadOnly,
            Cache = SqliteCacheMode.Shared,
        };
        var connection = new SqliteConnection(builder.ToString());
        connection.Open();
        return connection;
    }

    private static Stream OpenResource() =>
        typeof(DexDatabase).Assembly.GetManifestResourceStream(ResourceName)
        ?? throw new InvalidOperationException("O dex.sqlite não está embutido (rode scripts/build-dex.py).");

    private string Prepare()
    {
        string sha8;
        using (var stream = OpenResource())
            sha8 = Convert.ToHexStringLower(SHA256.HashData(stream))[..8];

        Directory.CreateDirectory(_folder);
        var target = Path.Combine(_folder, sha8 + ".sqlite");
        if (!File.Exists(target))
        {
            var tmp = target + ".tmp";
            using (var source = OpenResource())
            using (var output = File.Create(tmp))
                source.CopyTo(output);
            File.Move(tmp, target, overwrite: true);
        }

        foreach (var old in Directory.EnumerateFiles(_folder, "*.sqlite"))
        {
            if (string.Equals(Path.GetFullPath(old), Path.GetFullPath(target), StringComparison.Ordinal))
                continue;
            try { File.Delete(old); }
            catch (IOException) { } // outra instância ainda com a versão antiga aberta (Windows): fica para a próxima
            catch (UnauthorizedAccessException) { }
        }
        return Path.GetFullPath(target);
    }
}
