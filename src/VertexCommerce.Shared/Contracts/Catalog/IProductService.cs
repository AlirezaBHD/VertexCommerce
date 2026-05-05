namespace VertexCommerce.Shared.Contracts.Catalog;

public interface IProductService
{
    Task<ProductVariantInfo?> GetProductVariantInfoAsync(Guid productId, Guid variantId, CancellationToken ct = default);
}

public sealed record ProductVariantInfo(
    Guid ProductId,
    Guid VariantId,
    string Name,
    string Sku,
    string? ImagePath,
    decimal Price,
    int StockQuantity,
    List<ProductInfoAttribute> Attributes
);

public sealed record ProductInfoAttribute(
    string AttributeCode,
    string OptionCode);
