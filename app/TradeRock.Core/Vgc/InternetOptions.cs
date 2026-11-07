using System.Net;
using System.Net.Http.Headers;

namespace TradeRock.Core.Vgc;

/// <summary>
/// "Internet opcional" (D32): cada opção liga um download, e as duas começam desligadas. Nenhuma envia dados do PC:
/// só fazem GET de arquivos públicos (estatísticas do Smogon e o texto de um link do pokepast.es).
/// </summary>
public sealed record InternetSettings(bool Smogon = false, bool Pokepaste = false)
{
    public const string Key = "internet";

    public static readonly InternetSettings Default = new();
}

/// <summary>Falha de um download opcional. <see cref="Code"/> é curto e vai para a interface (sem URL nem detalhes).</summary>
public sealed class InternetException(string code) : Exception(code)
{
    public string Code { get; } = code;
}

/// <summary>O HttpClient dos downloads opcionais: só User-Agent, tempo limite e sem seguir redirecionamentos.</summary>
public static class InternetHttp
{
    /// <summary>Nome do HttpClient (os testes trocam o handler por este nome).</summary>
    public const string ClientName = "vgc-internet";

    public static readonly TimeSpan Timeout = TimeSpan.FromMinutes(2);

    public static void Configure(HttpClient http, string version)
    {
        http.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RockAssistent", version.Split('+')[0]));
        http.Timeout = Timeout;
    }

    /// <summary>Redirecionamento poderia levar a outro site: não segue (vira erro <c>http_3xx</c>).</summary>
    public static HttpMessageHandler CreateHandler() => new SocketsHttpHandler
    {
        AllowAutoRedirect = false,
        AutomaticDecompression = DecompressionMethods.All,
        UseCookies = false,
    };

    /// <summary>GET simples; erros de rede viram <see cref="InternetException"/> com código curto.</summary>
    internal static async Task<HttpResponseMessage> GetAsync(HttpClient http, Uri url, CancellationToken ct)
    {
        try
        {
            return await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new InternetException("timeout");
        }
        catch (HttpRequestException)
        {
            throw new InternetException("unreachable");
        }
    }

    /// <summary>
    /// Copia o corpo para <paramref name="target"/> com limite de tamanho (<c>too_big</c>) e de tempo: com
    /// <c>ResponseHeadersRead</c> o <see cref="HttpClient.Timeout"/> só cobre os cabeçalhos, então o corpo ganha o seu
    /// (<paramref name="timeout"/>, normalmente o do cliente). Falha ao gravar vira <c>write_failed</c>.
    /// </summary>
    internal static async Task CopyLimitedAsync(HttpResponseMessage response, Stream target, long maxBytes, TimeSpan timeout,
        CancellationToken ct)
    {
        if (response.Content.Headers.ContentLength is > 0 and var length && length > maxBytes)
            throw new InternetException("too_big");
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        if (timeout > TimeSpan.Zero && timeout != System.Threading.Timeout.InfiniteTimeSpan)
            cts.CancelAfter(timeout);
        try
        {
            await using var source = await response.Content.ReadAsStreamAsync(cts.Token);
            var buffer = new byte[81920];
            long total = 0;
            int read;
            while ((read = await source.ReadAsync(buffer, cts.Token)) > 0)
            {
                total += read;
                if (total > maxBytes)
                    throw new InternetException("too_big");
                try
                {
                    await target.WriteAsync(buffer.AsMemory(0, read), cts.Token);
                }
                catch (Exception e) when (e is IOException or UnauthorizedAccessException)
                {
                    throw new InternetException("write_failed");
                }
            }
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new InternetException("timeout");
        }
        catch (Exception e) when (e is HttpRequestException or IOException)
        {
            // Inclui o HttpIOException (conexão caiu no meio do corpo).
            throw new InternetException("unreachable");
        }
    }

    internal static string StatusCode(HttpResponseMessage response) => $"http_{(int)response.StatusCode}";
}
