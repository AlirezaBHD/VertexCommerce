using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById;
using VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById.DTOs;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetCatalogAttributes;

public sealed class
    GetCatalogAttributesQueryHandler(IProductRepository productRepository) : IQueryHandler<GetCatalogAttributesQuery,
    IReadOnlyList<CatalogAttributesResponse>>
{
    public async Task<Result<IReadOnlyList<CatalogAttributesResponse>>> Handle(GetCatalogAttributesQuery query,
        CancellationToken ct)
    {
        var spec = new GetAttributesSpec();
        var attributes = await productRepository.GetAttributes(spec, ct);
        return Result.Success(attributes);
    }
}
