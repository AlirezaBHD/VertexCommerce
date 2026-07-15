using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.ReorderBanners;

internal sealed class ReorderBannersCommandHandler(
    IContentRepository contentRepository)
    : ICommandHandler<ReorderBannersCommand>
{
    public async Task<Result> Handle(ReorderBannersCommand command, CancellationToken ct)
    {
        foreach (var item in command.Items)
        {
            await contentRepository.UpdateBannerSortOrderAsync(item.BannerId, item.SortOrder, ct);
        }

        return Result.Success();
    }
}
