using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Catalog.Domain.Products.Events;
using VertexCommerce.Modules.Catalog.Sync;
using VertexCommerce.Modules.Catalog.Sync.Products;

namespace VertexCommerce.Modules.Catalog.Features.Products.EventHandlers;

internal sealed class ProductDeletedEventHandler
    : INotificationHandler<ProductDeletedEvent>
{
    private readonly IProductSyncService _syncService;
    private readonly ILogger<ProductDeletedEventHandler> _logger;

    public ProductDeletedEventHandler(
        IProductSyncService syncService,
        ILogger<ProductDeletedEventHandler> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    public async Task Handle(
        ProductDeletedEvent notification,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Handling ProductDeletedEvent for {ProductId}. Removing from MongoDB...",
            notification.ProductId);

        try
        {
            await _syncService.DeleteProductAsync(notification.ProductId, cancellationToken);
        }
        catch (Exception ex)
        {
            // TODO: Outbox Pattern or Retry Queue
            _logger.LogError(ex,
                "Failed to delete product {ProductId} from MongoDB",
                notification.ProductId);
        }
    }
}
