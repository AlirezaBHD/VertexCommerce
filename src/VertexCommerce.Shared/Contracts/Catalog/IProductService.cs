namespace VertexCommerce.Shared.Contracts.Catalog;

public interface IProductService
{
    Task<ProductVariantInfo?> GetProductVariantInfoAsync(Guid productId, Guid variantId, CancellationToken ct = default);
}
