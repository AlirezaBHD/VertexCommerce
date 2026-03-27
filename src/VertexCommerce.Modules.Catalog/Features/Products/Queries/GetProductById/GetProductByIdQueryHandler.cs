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
        var product = await _productRepository.GetByIdAsync(query.Id, ct);

        if (product is null)
        {
            return Result.Failure<ProductResponse>(Error.NotFound("Product", query.Id));
        }

        var response = new ProductResponse(
            product.Id,
            product.Name,
            product.Description,
            product.IsActive,
            product.CategoryId,
            product.Category?.Name,
            product.CreatedAt,
            product.UpdatedAt,
            product.Attributes.Select(a => new ProductAttributeResponse(
                a.Key,
                a.Value,
                a.Type
            )).ToList()
        );

        return Result.Success(response);
    }
}
