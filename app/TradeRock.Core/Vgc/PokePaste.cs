using System.Text;
using System.Text.RegularExpressions;

namespace TradeRock.Core.Vgc;

/// <summary>
/// Abre um link do pokepast.es: só <c>https://pokepast.es/&lt;id&gt;</c> (host exato, porta padrão, sem usuário), baixa
/// <c>/&lt;id&gt;/raw</c> com limite de 20 KB e devolve o texto. Qualquer outro endereço vira <c>paste_host_not_allowed</c>.
/// </summary>
public static class PokePaste
{
    public const string Host = "pokepast.es";

    private static readonly Regex IdPattern = new("^[A-Za-z0-9]{1,64}$", RegexOptions.CultureInvariant);

    /// <summary>O endereço do texto do paste, ou nulo se o link não for do pokepast.es.</summary>
    public static Uri? RawUri(Uri? url)
    {
        if (url is null || !url.IsAbsoluteUri || url.Scheme != Uri.UriSchemeHttps
            || !string.Equals(url.Host, Host, StringComparison.OrdinalIgnoreCase) || !url.IsDefaultPort || url.UserInfo.Length > 0)
            return null;
        var parts = url.AbsolutePath.Trim('/').Split('/');
        if (parts.Length is < 1 or > 2 || !IdPattern.IsMatch(parts[0]) || parts.Length == 2 && parts[1] != "raw")
            return null;
        return new Uri($"https://{Host}/{parts[0]}/raw");
    }

    /// <summary>O link digitado (texto), ou nulo se não for um link do pokepast.es.</summary>
    public static Uri? RawUri(string? text) =>
        Uri.TryCreate(text?.Trim(), UriKind.Absolute, out var url) ? RawUri(url) : null;

    public static async Task<string> FetchAsync(HttpClient http, Uri url, CancellationToken ct)
    {
        var raw = RawUri(url) ?? throw new InternetException("paste_host_not_allowed");
        using var response = await InternetHttp.GetAsync(http, raw, ct);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            throw new InternetException("paste_not_found");
        if (!response.IsSuccessStatusCode)
            throw new InternetException(InternetHttp.StatusCode(response));
        using var buffer = new MemoryStream();
        try
        {
            await InternetHttp.CopyLimitedAsync(response, buffer, VgcPaste.MaxBytes, http.Timeout, ct);
        }
        catch (InternetException e) when (e.Code == "too_big")
        {
            throw new InternetException("paste_too_big");
        }
        // O raw tem \r\n; o analisador aceita os dois.
        return Encoding.UTF8.GetString(buffer.GetBuffer(), 0, (int)buffer.Length).Replace("\r\n", "\n");
    }
}
