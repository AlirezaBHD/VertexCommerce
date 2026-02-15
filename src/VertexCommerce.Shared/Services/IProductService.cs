namespace VertexCommerce.Shared.Services;

public interface IProductService
{
    Task<ProductInfo?> GetProductInfoAsync(Guid productId, CancellationToken ct = default);
}

public sealed record ProductInfo(
    Guid Id,
    string Name,
    string Sku,
    string? ImageUrl,
    decimal Price,
    string Currency,
    int StockQuantity,
    bool IsActive
);