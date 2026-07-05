using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Persistence.Postgres;
using VertexCommerce.Shared.Contracts.Catalog;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Services;

internal sealed class StockService(
    IProductRepository productRepository,
    ICatalogUnitOfWork unitOfWork) : IStockService
{
    public async Task<Result> DeductStockAsync(Guid variantId, int quantity, CancellationToken ct = default)
    {
        var variant = await productRepository.GetVariantByIdAsync(variantId, ct);

        if (variant is null)
            return Result.Failure(Error.NotFound("ProductVariant", variantId));

        if (!variant.TryDeductStock(quantity))
            return Result.Failure(Error.Validation(
                "Stock.Insufficient",
                $"Insufficient stock for variant {variantId}. Requested: {quantity}, Available: {variant.StockQuantity}"));

        productRepository.UpdateVariantAsync(variant);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
