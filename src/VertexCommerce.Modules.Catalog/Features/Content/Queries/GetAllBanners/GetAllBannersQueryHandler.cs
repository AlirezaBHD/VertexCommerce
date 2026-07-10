using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Queries.GetAllBanners;

internal sealed class GetAllBannersQueryHandler(IContentRepository contentRepository)
    : IQueryHandler<GetAllBannersQuery, IReadOnlyList<BannerDocument>>
{
    public async Task<Result<IReadOnlyList<BannerDocument>>> Handle(
        GetAllBannersQuery query, CancellationToken ct)
    {
        var items = await contentRepository.GetAllBannersAsync(ct);
        return Result.Success(items);
    }
}
