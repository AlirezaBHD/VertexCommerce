using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Catalog.Domain.Categories.Events;
using VertexCommerce.Modules.Catalog.Sync.Categories;

namespace VertexCommerce.Modules.Catalog.Features.Categories.EventHandlers;

internal sealed class CategoryUpdatedEventHandler
    : INotificationHandler<CategoryUpdatedEvent>
{
    private readonly CategorySyncService _syncService;
    private readonly ILogger<CategoryUpdatedEventHandler> _logger;

    public CategoryUpdatedEventHandler(
        CategorySyncService syncService,
        ILogger<CategoryUpdatedEventHandler> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    public async Task Handle(
        CategoryUpdatedEvent notification, CancellationToken ct)
    {
        _logger.LogInformation(
            "Syncing updated category {CategoryId} to MongoDB",
            notification.CategoryId);

        await _syncService.SyncCategoryAsync(notification.CategoryId, ct);
    }
}
