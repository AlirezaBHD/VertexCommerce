using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Queries.GetAllHero;

internal sealed class GetAllHeroQueryHandler(IContentRepository contentRepository)
    : IQueryHandler<GetAllHeroQuery, IReadOnlyList<HeroContentDocument>>
{
    public async Task<Result<IReadOnlyList<HeroContentDocument>>> Handle(
        GetAllHeroQuery query, CancellationToken ct)
    {
        var items = await contentRepository.GetAllHeroAsync(ct);
        return Result.Success(items);
    }
}
