using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Queries.Lookups;

internal sealed class GetCategoryLookupQueryHandler(ICategoryReadModelRepository repository)
    : IQueryHandler<GetCategoryLookupQuery, IReadOnlyList<CategoryLookupItem>>
{
    public async Task<Result<IReadOnlyList<CategoryLookupItem>>> Handle(GetCategoryLookupQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 50);
        var categories = await repository.SearchAsync(request.SearchTerm, limit, cancellationToken);

        return categories.Select(c => new CategoryLookupItem(
            c.Id,
            c.Name,
            c.Slug,
            c.Breadcrumb
        )).ToList();
    }
}
