using MediatR;
using Microsoft.Extensions.Logging;
using VertexCommerce.Modules.Catalog.Domain.Categories.Events;
using VertexCommerce.Modules.Catalog.Sync.Categories;

namespace VertexCommerce.Modules.Catalog.Features.Categories.EventHandlers;

internal sealed class CategoryDeletedEventHandler(
    ICategorySyncService syncService,
    ILogger<CategoryDeletedEventHandler> logger)
    : INotificationHandler<CategoryDeletedEvent>
{
    public async Task Handle(
        CategoryDeletedEvent notification, CancellationToken ct)
    {
        logger.LogInformation(
            "Deleting category {CategoryId} from MongoDB",
            notification.CategoryId);

        await syncService.DeleteCategoryAsync(notification.CategoryId, ct);
    }
}
