using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace VertexCommerce.Shared.Persistence.Outbox;

public sealed class OutboxProcessor<TDbContext>(
    IServiceScopeFactory scopeFactory,
    OutboxSignal signal,
    OutboxProcessorOptions options,
    ILogger<OutboxProcessor<TDbContext>> logger)
    : BackgroundService
    where TDbContext : DbContext, IOutboxDbContext
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types",
        Justification = "Background worker loop must log unexpected batch errors and continue running rather than crashing the host process.")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Outbox processor started for {Module}", options.ModuleName);

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
                logger.LogError(ex, "Outbox processor error for {Module}", typeof(TDbContext).Name);
            }

            try
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                cts.CancelAfter(options.PollingInterval);
                await signal.WaitAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                // Expected when signal wait times out (polling interval elapsed) or service cancellation is requested.
            }
        }
    }

    [SuppressMessage("Design", "CA1031:Do not catch general exception types",
        Justification = "Per-message processing exceptions must be captured and recorded on the outbox entity for retry and dead-lettering.")]
    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();
        var publisher = scope.ServiceProvider.GetRequiredService<IPublisher>();

        var messages = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOn == null)
            .OrderBy(m => m.OccurredOn)
            .Take(options.BatchSize)
            .ToListAsync(ct);

        if (messages.Count == 0) return;

        foreach (var message in messages)
        {
            try
            {
                var notification = OutboxSerializer.Deserialize(message.Type, message.Content);

                if (notification is not null)
                {
                    await publisher.Publish(notification, ct);
                }

                message.ProcessedOn = DateTime.UtcNow;
                message.Error = null;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "Failed to process outbox message {MessageId} (attempt {Retry})",
                    message.Id, message.RetryCount + 1);

                message.RetryCount++;
                message.Error = ex.Message.Length > 2000
                    ? ex.Message[..2000]
                    : ex.Message;

                if (message.RetryCount >= options.MaxRetryCount)
                {
                    message.ProcessedOn = DateTime.UtcNow;
                    logger.LogError("Outbox message {MessageId} dead-lettered after {Max} retries",
                        message.Id, options.MaxRetryCount);
                }
            }
        }

        await dbContext.SaveChangesAsync(ct);
    }
}
