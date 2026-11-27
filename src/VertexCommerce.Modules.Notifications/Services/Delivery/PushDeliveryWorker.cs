using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace VertexCommerce.Modules.Notifications.Services.Delivery;

internal sealed class PushDeliveryWorker(
    IServiceScopeFactory scopeFactory, IOptions<WebPushOptions> options, ILogger<PushDeliveryWorker> logger) : BackgroundService
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types",
        Justification = "A storage failure must leave queued jobs recoverable and keep the worker running.")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Value.IsEnabled)
        {
            return;
        }

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await using var scope = scopeFactory.CreateAsyncScope();
                    var processor = scope.ServiceProvider.GetRequiredService<PushDeliveryProcessor>();

                    if (await processor.ProcessNextAsync(stoppingToken))
                    {
                        continue;
                    }
                }
                catch (Exception ex) when (!stoppingToken.IsCancellationRequested)
                {
                    logger.LogError(ex, "Web Push queue processing failed");
                }

                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            // The next worker can reclaim an interrupted attempt after its lease expires.
        }
    }
}
