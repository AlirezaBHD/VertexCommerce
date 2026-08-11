using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Catalog.Domain.Categories.Events;
using VertexCommerce.Modules.Catalog.Sync.Categories;

namespace VertexCommerce.Modules.Catalog.Features.Categories.EventHandlers;

internal sealed class CategoryUpdatedEventHandler(
    ICategorySyncService syncService,
    ILogger<CategoryUpdatedEventHandler> logger)
    : INotificationHandler<CategoryUpdatedEvent>
{
    public async Task Handle(
        CategoryUpdatedEvent notification, CancellationToken ct)
    {
        logger.LogInformation(
            "Syncing updated category {CategoryId} to MongoDB",
            notification.CategoryId);

        await syncService.SyncCategoryAsync(notification.CategoryId, ct);
    }
}
