using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TradeRock.Core.Games;
using TradeRock.Core.Storage;
using TradeRock.Core.Trading;

namespace TradeRock.Core.Activities;

public enum ActivityStatus
{
    Running,
    Completed,
    Failed,
    Cancelled,
}

public sealed record ActivitySnapshot(
    Guid Id,
    TradeGame Game,
    ActivityKind Kind,
    string? GiftLabel,
    ActivityStatus Status,
    DateTimeOffset StartedAt,
    DateTimeOffset? FinishedAt,
    IReadOnlyList<ActivityEvent> Events,
    string? ErrorCode,
    string? ErrorMessage,
    string? RaidKind = null);

/// <summary>Já há uma troca ou outra atividade usando o rádio.</summary>
public sealed class RadioBusyException() : Exception("O rádio está ocupado.");

/// <summary>
/// Uma atividade por vez, sem fila (D26): a pessoa acompanha pela tela e cancela por ela. O rádio é o mesmo da
/// troca, então uma não começa enquanto a outra roda. No fim (sucesso, cancelamento ou erro) grava um
/// <see cref="ActivityRecord"/> para o Histórico.
/// </summary>
public sealed class ActivityService(
    TradeCoordinator trades, IDbContextFactory<TradeRockDb> dbFactory, TimeProvider time, ILogger<ActivityService> logger)
{
    private readonly Lock _lock = new();
    private Session? _session;

    public bool IsBusy
    {
        get { lock (_lock) return _session is { Status: ActivityStatus.Running }; }
    }

    /// <summary>A atividade atual ou a última (null se nenhuma nesta execução).</summary>
    public ActivitySnapshot? Current
    {
        get { lock (_lock) return _session?.Snapshot(); }
    }

    public ActivitySnapshot Start(IActivityEngine engine, ActivityRequest request, string? giftLabel)
    {
        Session session;
        lock (_lock)
        {
            if (_session is { Status: ActivityStatus.Running } || trades.IsBusy)
                throw new RadioBusyException();
            // O dono é lido aqui: dentro do Task.Run o AsyncLocal da requisição não vale mais.
            session = new Session(Guid.NewGuid(), request, giftLabel, time.GetUtcNow(), OwnerScope.Current,
                (engine as ITradeEngine)?.Id ?? "unknown", (engine as ITradeEngine)?.IsSimulation ?? false);
            _session = session;
        }
        _ = Task.Run(() => RunAsync(engine, session));
        return session.Snapshot();
    }

    public bool Cancel()
    {
        lock (_lock)
        {
            if (_session is not { Status: ActivityStatus.Running } s)
                return false;
            s.Cancel.Cancel();
            return true;
        }
    }

    private async Task RunAsync(IActivityEngine engine, Session s)
    {
        ActivityResult result;
        try
        {
            result = await engine.RunActivityAsync(s.Request, e =>
            {
                lock (_lock)
                    s.Events.Add(e);
                return ValueTask.CompletedTask;
            }, s.Cancel.Token);
        }
        catch (Exception e) when (e is not OperationCanceledException)
        {
            logger.LogError(e, "Atividade {Id} parou com erro.", s.Id);
            result = ActivityResult.Fail("engine_error", e.Message);
        }
        catch (OperationCanceledException)
        {
            result = ActivityResult.Cancelled;
        }
        var status = result.Ok ? ActivityStatus.Completed : result.Code == "cancelled" ? ActivityStatus.Cancelled : ActivityStatus.Failed;
        var errorCode = result.Ok || status == ActivityStatus.Cancelled ? null : result.Code;
        var finishedAt = time.GetUtcNow();

        // Grava antes de publicar o fim: quem acompanha /current só vê o fim com a linha já no banco.
        await RecordAsync(s, status, errorCode, finishedAt);

        lock (_lock)
        {
            s.Status = status;
            s.ErrorCode = errorCode;
            s.ErrorMessage = errorCode is null ? null : result.Message;
            s.FinishedAt = finishedAt;
            s.Events.Add(new ActivityEvent(ActivityEventKind.Finished, finishedAt));
        }
        s.Cancel.Dispose();
    }

    /// <summary>Registro para o Histórico. Uma falha aqui só vira aviso no log: a sessão termina do mesmo jeito.</summary>
    private async Task RecordAsync(Session s, ActivityStatus status, string? errorCode, DateTimeOffset finishedAt)
    {
        try
        {
            var r = s.Request;
            var (kind, subKind, species, summary) = r.Kind switch
            {
                ActivityKind.Raid => ("raid", r.Raid?.Kind, r.Raid?.Legendary, (object?)(r.Raid is { } raid
                    ? new { @event = raid.Event, eventBoss = raid.EventBoss, players = raid.Players, gigantamax = raid.Gigantamax }
                    : null)),
                ActivityKind.Circle => ("circle", r.Circle?.Kind, r.Circle?.Species, r.Circle is { } c
                    ? new { task = c.Task, bp = c.Bp, level = c.Level, shiny = c.Shiny, tera = c.Tera }
                    : null),
                // Nunca o arquivo do presente (Data): só como ele foi escolhido.
                _ => ("gift", null, null, r.Gift is { } g ? new { mode = g.Mode, key = g.Key, extension = g.Extension } : null),
            };
            using var scope = OwnerScope.Use(s.OwnerId);
            await using var db = await dbFactory.CreateDbContextAsync();
            db.ActivityRecords.Add(new ActivityRecord
            {
                Id = s.Id,
                OwnerId = s.OwnerId,
                Kind = kind,
                SubKind = subKind,
                Game = r.Game,
                EngineId = s.EngineId,
                IsSimulation = s.IsSimulation,
                StartedAt = s.StartedAt,
                EndedAt = finishedAt,
                Status = status switch
                {
                    ActivityStatus.Completed => "completed",
                    ActivityStatus.Cancelled => "cancelled",
                    _ => "failed",
                },
                Ok = status == ActivityStatus.Completed,
                ErrorCode = errorCode,
                Label = s.GiftLabel,
                Species = species,
                SummaryJson = JsonSerializer.Serialize(summary ?? new { }),
            });
            await db.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "Não foi possível gravar a atividade {Id} no histórico.", s.Id);
        }
    }

    private sealed class Session(Guid id, ActivityRequest request, string? giftLabel, DateTimeOffset startedAt,
        Guid ownerId, string engineId, bool isSimulation)
    {
        public Guid Id { get; } = id;
        public string? GiftLabel { get; } = giftLabel;
        public DateTimeOffset StartedAt { get; } = startedAt;
        public Guid OwnerId { get; } = ownerId;
        public string EngineId { get; } = engineId;
        public bool IsSimulation { get; } = isSimulation;
        public ActivityRequest Request { get; } = request;
        public CancellationTokenSource Cancel { get; } = new();
        public List<ActivityEvent> Events { get; } = [];
        public ActivityStatus Status { get; set; } = ActivityStatus.Running;
        public DateTimeOffset? FinishedAt { get; set; }
        public string? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }

        public ActivitySnapshot Snapshot() => new(Id, Request.Game, Request.Kind, GiftLabel, Status, StartedAt, FinishedAt,
            [.. Events], ErrorCode, ErrorMessage, Request.Raid?.Kind ?? Request.Circle?.Kind);
    }
}
