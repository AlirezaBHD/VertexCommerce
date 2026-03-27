using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Catalog.Domain.Products.Events;
using VertexCommerce.Modules.Catalog.Sync;

namespace VertexCommerce.Modules.Catalog.Features.Products.EventHandlers;

internal sealed class ProductCreatedEventHandler
    : INotificationHandler<ProductCreatedEvent>
{
    private readonly IProductSyncService _syncService;
    private readonly ILogger<ProductCreatedEventHandler> _logger;

    public ProductCreatedEventHandler(
        IProductSyncService syncService,
        ILogger<ProductCreatedEventHandler> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    public async Task Handle(
        ProductCreatedEvent notification,
        CancellationToken cancellationToken)
    {
        
        _logger.LogInformation(
            "Handling ProductCreatedEvent for {ProductId} ({ProductName}). Syncing to MongoDB...",
            notification.ProductId,
            notification.Name);

        try
        {
            await _syncService.SyncProductAsync(notification.ProductId, cancellationToken);
        }
        catch (Exception ex)
        {
            // TODO: Outbox Pattern or Retry Queue
            _logger.LogError(ex,
                "Failed to sync product {ProductId} to MongoDB. Data will be inconsistent until next sync",
                notification.ProductId);
        }
    }
}
