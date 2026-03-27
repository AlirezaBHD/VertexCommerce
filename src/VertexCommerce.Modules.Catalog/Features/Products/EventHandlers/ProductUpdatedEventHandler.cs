using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Catalog.Domain.Products.Events;
using VertexCommerce.Modules.Catalog.Sync;

namespace VertexCommerce.Modules.Catalog.Features.Products.EventHandlers;

internal sealed class ProductUpdatedEventHandler
    : INotificationHandler<ProductUpdatedEvent>
{
    private readonly IProductSyncService _syncService;
    private readonly ILogger<ProductUpdatedEventHandler> _logger;

    public ProductUpdatedEventHandler(
        IProductSyncService syncService,
        ILogger<ProductUpdatedEventHandler> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    public async Task Handle(
        ProductUpdatedEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling ProductUpdatedEvent for {ProductId}. Syncing to MongoDB...",
            notification.ProductId);

        try
        {
            await _syncService.SyncProductAsync(notification.ProductId, cancellationToken);
        }
        catch (Exception ex)
        {
            // TODO: Outbox Pattern or Retry Queue
            _logger.LogError(ex,
                "Failed to sync updated product {ProductId} to MongoDB",
                notification.ProductId);
        }
    }
}
