using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Background read-model synchronization error boundary to prevent failing primary database transactions")]
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
