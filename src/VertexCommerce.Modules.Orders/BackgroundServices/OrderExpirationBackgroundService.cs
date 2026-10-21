using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts.Catalog;

namespace VertexCommerce.Modules.Orders.BackgroundServices;

public class OrderExpirationBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<OrderExpirationBackgroundService> logger) : BackgroundService
{
    [SuppressMessage("Design", "CA1031: Modify 'ExecuteAsync' to catch a more specific allowed exception type, or rethrow the exception", 
        Justification = "At the BackgroundService level, catching all errors is essential to keep the loop alive.")]
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));

        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await ProcessExpiredOrdersAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            // 
            // #pragma warning disable CA1031
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while processing expired orders.");
            }
        }
    }

    private async Task ProcessExpiredOrdersAsync(CancellationToken ct)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IOrdersUnitOfWork>();
        var stockService = scope.ServiceProvider.GetRequiredService<IStockService>();

        var pendingOrders = await orderRepository.GetExpiredOrdersAsync(DateTime.UtcNow, ct);

        if (!pendingOrders.Any())
            return;

        foreach (var order in pendingOrders)
        {
            logger.LogInformation("Expiring order {OrderId} ({OrderNumber})", order.Id, order.OrderNumber);
            
            var expireResult = order.Expire();
            if (expireResult.IsSuccess)
            {
                var stockRequests = order.Items.Select(i => new StockDeductionRequest(i.VariantId, i.Quantity));
                var releaseResult = await stockService.ReleaseStocksAsync(stockRequests, ct);
                
                if (releaseResult.IsFailure)
                {
                    logger.LogWarning("Failed to release stock for expired order {OrderId}: {Error}", order.Id, releaseResult.Error);
                }
            }
            else
            {
                logger.LogWarning("Failed to expire order {OrderId}: {Error}", order.Id, expireResult.Error);
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
    }
}

