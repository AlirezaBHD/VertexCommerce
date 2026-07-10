using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Features.Products.Queries.GetCatalogAttributes;
using VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById.DTOs;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler(IProductRepository productRepository)
    : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var spec = new GetProductByIdSpec(query.Id);
        var product = await productRepository.GetByIdAsync(query.Id, spec, ct);

        if (product is null)
        {
            return Result.Failure<ProductResponse>(Error.NotFound("Product", query.Id));
        }


        return Result.Success(product);
    }
}
