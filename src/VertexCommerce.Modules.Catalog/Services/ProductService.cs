using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products;
using VertexCommerce.Shared.Contracts.Catalog;

namespace VertexCommerce.Modules.Catalog.Services;
internal sealed class ProductService(IProductReadModelRepository repository) 
    : IProductService
{
    public async Task<ProductVariantInfo?> GetProductVariantInfoAsync(
        Guid productId, 
        Guid variantId, 
        CancellationToken ct = default)
    {
        return await repository.GetProductVariantInfoAsync(productId, variantId, ct);
    }
}
