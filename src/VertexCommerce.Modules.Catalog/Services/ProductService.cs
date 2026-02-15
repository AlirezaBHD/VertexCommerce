using VertexCommerce.Modules.Catalog.ReadModels;
using VertexCommerce.Shared.Services;

namespace VertexCommerce.Modules.Catalog.Services;

internal sealed class ProductService : IProductService
{
    private readonly IProductReadModelRepository _repository;

    public ProductService(IProductReadModelRepository repository)
    {
        _repository = repository;
    }

    public async Task<ProductInfo?> GetProductInfoAsync(Guid productId, CancellationToken ct = default)
    {
        var product = await _repository.GetByIdAsync(productId, ct);

        if (product is null)
            return null;

        return new ProductInfo(
            product.Id,
            product.Name,
            product.Sku,
            null,
            product.Price,
            product.Currency,
            product.StockQuantity,
            product.IsActive
        );
    }
}
