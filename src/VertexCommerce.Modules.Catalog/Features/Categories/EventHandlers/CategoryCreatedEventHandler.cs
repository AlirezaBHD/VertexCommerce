using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Catalog.Domain.Categories.Events;
using VertexCommerce.Modules.Catalog.Sync.Categories;

namespace VertexCommerce.Modules.Catalog.Features.Categories.EventHandlers;

internal sealed class CategoryCreatedEventHandler
    : INotificationHandler<CategoryCreatedEvent>
{
    private readonly CategorySyncService _syncService;
    private readonly ILogger<CategoryCreatedEventHandler> _logger;

    public CategoryCreatedEventHandler(
        CategorySyncService syncService,
        ILogger<CategoryCreatedEventHandler> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    public async Task Handle(
        CategoryCreatedEvent notification, CancellationToken ct)
    {
        _logger.LogInformation(
            "Syncing created category {CategoryId} to MongoDB",
            notification.CategoryId);

        await _syncService.SyncCategoryAsync(notification.CategoryId, ct);
    }
}
