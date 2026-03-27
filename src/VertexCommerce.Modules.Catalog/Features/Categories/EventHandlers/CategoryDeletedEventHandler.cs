using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Catalog.Domain.Categories.Events;
using VertexCommerce.Modules.Catalog.Sync.Categories;

namespace VertexCommerce.Modules.Catalog.Features.Categories.EventHandlers;

internal sealed class CategoryDeletedEventHandler
    : INotificationHandler<CategoryDeletedEvent>
{
    private readonly CategorySyncService _syncService;
    private readonly ILogger<CategoryDeletedEventHandler> _logger;

    public CategoryDeletedEventHandler(
        CategorySyncService syncService,
        ILogger<CategoryDeletedEventHandler> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    public async Task Handle(
        CategoryDeletedEvent notification, CancellationToken ct)
    {
        _logger.LogInformation(
            "Deleting category {CategoryId} from MongoDB",
            notification.CategoryId);

        await _syncService.DeleteCategoryAsync(notification.CategoryId, ct);
    }
}
