using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById;

public sealed class GetProductByIdQueryHandler : IQueryHandler<GetProductByIdQuery, ProductResponse>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<Result<ProductResponse>> Handle(GetProductByIdQuery query, CancellationToken ct)
    {
        var spec = new ProductByIdSpec(query.Id);
        var product = await _productRepository.GetByIdAsync(query.Id, spec, ct);

        if (product is null)
        {
            return Result.Failure<ProductResponse>(Error.NotFound("Product", query.Id));
        }


        return Result.Success(product);
    }
}
