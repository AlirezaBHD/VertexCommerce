using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Orders.Domain.Enums;
using VertexCommerce.Modules.Orders.Domain.Repositories;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts.Catalog;

namespace VertexCommerce.Modules.Orders.BackgroundServices;

public class OrderExpirationBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<OrderExpirationBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessExpiredOrdersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while processing expired orders.");
            }

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }

    private async Task ProcessExpiredOrdersAsync(CancellationToken ct)
    {
        using var scope = serviceProvider.CreateScope();
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

