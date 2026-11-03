using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Catalog.Domain.Products.Events;
using VertexCommerce.Modules.Catalog.Sync;
using VertexCommerce.Modules.Catalog.Sync.Products;

namespace VertexCommerce.Modules.Catalog.Features.Products.EventHandlers;

internal sealed class ProductUpdatedEventHandler(
    IProductSyncService syncService,
    ILogger<ProductUpdatedEventHandler> logger)
    : INotificationHandler<ProductUpdatedEvent>
{
    public async Task Handle(
        ProductUpdatedEvent notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handling ProductUpdatedEvent for {ProductId}. Syncing to MongoDB...",
            notification.ProductId);

        try
        {
            await syncService.SyncProductAsync(notification.ProductId, cancellationToken);
        }
        catch (Exception ex)
        {
            // TODO: Outbox Pattern or Retry Queue
            logger.LogError(ex,
                "Failed to sync updated product {ProductId} to MongoDB",
                notification.ProductId);
        }
    }
}
