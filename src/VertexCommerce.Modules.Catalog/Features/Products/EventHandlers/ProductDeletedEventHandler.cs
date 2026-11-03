using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Catalog.Domain.Products.Events;
using VertexCommerce.Modules.Catalog.Sync;
using VertexCommerce.Modules.Catalog.Sync.Products;

namespace VertexCommerce.Modules.Catalog.Features.Products.EventHandlers;

internal sealed class ProductDeletedEventHandler(
    IProductSyncService syncService,
    ILogger<ProductDeletedEventHandler> logger)
    : INotificationHandler<ProductDeletedEvent>
{
    public async Task Handle(
        ProductDeletedEvent notification,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Handling ProductDeletedEvent for {ProductId}. Removing from MongoDB...",
            notification.ProductId);

        try
        {
            await syncService.DeleteProductAsync(notification.ProductId, cancellationToken);
        }
        catch (Exception ex)
        {
            // TODO: Outbox Pattern or Retry Queue
            logger.LogError(ex,
                "Failed to delete product {ProductId} from MongoDB",
                notification.ProductId);
        }
    }
}
