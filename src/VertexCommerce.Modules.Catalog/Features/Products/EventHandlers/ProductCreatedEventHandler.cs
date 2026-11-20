using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Catalog.Domain.Products.Events;
using VertexCommerce.Modules.Catalog.Sync;
using VertexCommerce.Modules.Catalog.Sync.Products;

namespace VertexCommerce.Modules.Catalog.Features.Products.EventHandlers;

internal sealed class ProductCreatedEventHandler(
    IProductSyncService syncService,
    ILogger<ProductCreatedEventHandler> logger)
    : INotificationHandler<ProductCreatedEvent>
{
    [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Background read-model synchronization error boundary to prevent failing primary database transactions")]
    public async Task Handle(
        ProductCreatedEvent notification,
        CancellationToken cancellationToken)
    {

        logger.LogInformation(
            "Handling ProductCreatedEvent for {ProductId} ({ProductName}). Syncing to MongoDB...",
            notification.ProductId,
            notification.Name);

        try
        {
            await syncService.SyncProductAsync(notification.ProductId, cancellationToken);
        }
        catch (Exception ex)
        {
            // TODO: Outbox Pattern or Retry Queue
            logger.LogError(ex,
                "Failed to sync product {ProductId} to MongoDB. Data will be inconsistent until next sync",
                notification.ProductId);
        }
    }
}
