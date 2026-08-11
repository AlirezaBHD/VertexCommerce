using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Catalog.Domain.Categories.Events;
using VertexCommerce.Modules.Catalog.Sync.Categories;

namespace VertexCommerce.Modules.Catalog.Features.Categories.EventHandlers;

internal sealed class CategoryCreatedEventHandler(
    ICategorySyncService syncService,
    ILogger<CategoryCreatedEventHandler> logger)
    : INotificationHandler<CategoryCreatedEvent>
{
    public async Task Handle(
        CategoryCreatedEvent notification, CancellationToken ct)
    {
        logger.LogInformation(
            "Syncing created category {CategoryId} to MongoDB",
            notification.CategoryId);

        await syncService.SyncCategoryAsync(notification.CategoryId, ct);
    }
}
