using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace VertexCommerce.Shared.Persistence.Outbox;

public sealed class OutboxProcessor<TDbContext> : BackgroundService
    where TDbContext : DbContext, IOutboxDbContext
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly OutboxSignal _signal;
    private readonly OutboxProcessorOptions _options;
    private readonly ILogger<OutboxProcessor<TDbContext>> _logger;

    private static readonly JsonSerializerSettings DeserializeSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    public OutboxProcessor(
        IServiceScopeFactory scopeFactory,
        OutboxSignal signal,
        OutboxProcessorOptions options,
        ILogger<OutboxProcessor<TDbContext>> logger)
    {
        _scopeFactory = scopeFactory;
        _signal = signal;
        _options = options;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox processor started for {Module}", _options.ModuleName);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Outbox processor error for {Module}", typeof(TDbContext).Name);
            }

            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                cts.CancelAfter(_options.PollingInterval);
                await _signal.WaitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
            }
        }
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        var messages = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(_options.BatchSize)
            .ToListAsync(ct);

        if (messages.Count == 0) return;

        foreach (var message in messages)
        {
            try
            {
                var domainEvent = JsonConvert.DeserializeObject(
                    message.Content, DeserializeSettings);

                if (domainEvent is INotification notification)
                {
                    await publisher.Publish(notification, ct);
                }

                message.ProcessedOn = DateTime.UtcNow;
                message.Error = null;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Failed to process outbox message {MessageId} (attempt {Retry})",
                    message.Id, message.RetryCount + 1);

                message.RetryCount++;
                message.Error = ex.Message.Length > 2000
                    ? ex.Message[..2000]
                    : ex.Message;

                if (message.RetryCount >= _options.MaxRetryCount)
                {
                    message.ProcessedOn = DateTime.UtcNow;
                    _logger.LogError("Outbox message {MessageId} dead-lettered after {Max} retries",
                        message.Id, _options.MaxRetryCount);
                }
            }
        }

        await dbContext.SaveChangesAsync(ct);
    }
}
