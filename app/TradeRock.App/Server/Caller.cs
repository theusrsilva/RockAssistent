using TradeRock.Core.Storage;

namespace TradeRock.App.Server;

/// <summary>Quem fez a requisição: a janela local (sempre o dono do PC, administrador) ou uma conta pela rede.</summary>
public sealed record Caller(Guid UserId, string? Username, UserRole Role, bool IsLocal, LanSession? Session = null)
{
    public static readonly Caller LocalWindow = new(OwnerScope.LocalOwner, null, UserRole.Admin, IsLocal: true);

    public bool IsAdmin => Role == UserRole.Admin;

    public static Caller Of(HttpContext context) =>
        context.Items[typeof(Caller)] as Caller ?? throw new InvalidOperationException("Requisição sem identificação.");

    public static Caller? Find(HttpContext context) => context.Items[typeof(Caller)] as Caller;
}

/// <summary>
/// O que cada tipo de acesso pode fazer pela rede (modo servidor). A janela local pode tudo.
/// <list type="bullet">
/// <item>Só na janela local: o que não pode passar pela rede (o prod.keys nunca trafega, regra de segurança), o que pede a
/// senha de administrador na tela do PC (instalar o worker, instalar a atualização) e os diálogos nativos (abririam no PC;
/// pela rede a tela usa upload e download).</item>
/// <item>Só administradores: o que mexe no PC, no rádio ou nas configurações (conexão, motor, atividades, Boxes com os
/// saves do PC, logs, atualização, internet opcional, contas, desligar o modo servidor, fechar o app).</item>
/// <item>Só a conta do dono do PC: as teclas do atalho global do Contador Hunt (somam na hunt do dono).</item>
/// </list>
/// </summary>
public static class AccessPolicy
{
    /// <summary>Rotas que não precisam de login pela rede.</summary>
    public static bool IsPublic(HttpRequest request) =>
        !request.Path.StartsWithSegments("/api")
        || request.Path.StartsWithSegments("/api/health")
        || request.Path.StartsWithSegments("/api/auth");

    /// <summary>Nunca pela rede, nem para administradores.</summary>
    public static bool IsLocalOnly(HttpRequest request)
    {
        var path = request.Path;
        // prod.keys: enviar ou apagar só no PC (o arquivo nunca trafega pela rede). Ler o estado vale.
        if (path.StartsWithSegments("/api/setup/keys") && !IsSafeMethod(request.Method))
            return true;
        // Pedem a senha de administrador na tela do PC (pkexec/polkit, UAC).
        if (path.StartsWithSegments("/api/worker/install") || path.StartsWithSegments("/api/worker/uninstall")
            || path.StartsWithSegments("/api/update/install"))
            return true;
        // Diálogos nativos: abririam no PC. "/api/saves/open" não cobre "/api/saves/open-folder" (segmento diferente).
        return path.StartsWithSegments("/api/saves/open") || path.StartsWithSegments("/api/saves/open-folder")
            || path.StartsWithSegments("/api/vault/export/native");
    }

    /// <summary>Pela rede, só administradores.</summary>
    public static bool IsAdminOnly(HttpRequest request)
    {
        var path = request.Path;
        if (path.StartsWithSegments("/api/accounts") || path.StartsWithSegments("/api/setup") || path.StartsWithSegments("/api/worker")
            || path.StartsWithSegments("/api/server") || path.StartsWithSegments("/api/app/close")
            || path.StartsWithSegments("/api/activities") || path.StartsWithSegments("/api/logs")
            || path.StartsWithSegments("/api/update") || path.StartsWithSegments("/api/boxes")
            || path.StartsWithSegments("/api/settings/save-folders") || path.StartsWithSegments("/api/fs")
            // Internet opcional (D32): baixar e mudar a configuração. Ler o que já foi baixado vale para todos.
            || path.StartsWithSegments("/api/vgc/usage/refresh") || path.StartsWithSegments("/api/vgc/paste"))
            return true;
        return HttpMethods.IsPut(request.Method)
            && (path.StartsWithSegments("/api/engines/selected") || path.StartsWithSegments("/api/settings/trade")
                || path.StartsWithSegments("/api/settings/internet"));
    }

    /// <summary>Pela rede, só a conta do dono do PC: as teclas do atalho global somam na hunt aberta do dono.</summary>
    public static bool IsOwnerOnly(HttpRequest request) => request.Path.StartsWithSegments("/api/hunts/events");

    public static bool IsSafeMethod(string method) =>
        HttpMethods.IsGet(method) || HttpMethods.IsHead(method) || HttpMethods.IsOptions(method);
}
