using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.Lookups;

internal sealed class GetProductLookupQueryHandler(IProductReadModelRepository repository)
    : IQueryHandler<GetProductLookupQuery, IReadOnlyList<ProductLookupItem>>
{
    public async Task<Result<IReadOnlyList<ProductLookupItem>>> Handle(GetProductLookupQuery request, CancellationToken cancellationToken)
    {
        var limit = Math.Clamp(request.Limit, 1, 50);
        var products = await repository.SearchAsync(request.SearchTerm, limit, cancellationToken);

        return products.Select(p => new ProductLookupItem(
            p.Id,
            p.Name,
            p.Slug,
            p.Media.Count > 0 ? p.Media[0].Path : null
        )).ToList();
    }
}