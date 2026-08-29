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
    public async Task<Result> DeductStocksAsync(IEnumerable<StockDeductionRequest> requests, CancellationToken ct = default)
    {
        var variantsToUpdate = new List<ProductVariant>();

        foreach (var req in requests)
        {
            var variant = await productRepository.GetVariantByIdAsync(req.VariantId, ct);
            if (variant is null)
                return Result.Failure(Error.NotFound("ProductVariant", req.VariantId));

            if (!variant.TryDeductStock(req.Quantity))
                return Result.Failure(Error.Validation(
                    "Stock.Insufficient",
                    $"Insufficient stock for variant {req.VariantId}. Requested: {req.Quantity}, Available: {variant.StockQuantity}"));

            variantsToUpdate.Add(variant);
        }

        foreach (var variant in variantsToUpdate)
        {
            productRepository.UpdateVariantAsync(variant);
        }

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }

    public async Task<Result> ReserveStocksAsync(IEnumerable<StockDeductionRequest> requests, CancellationToken ct = default)
    {
        var variantsToUpdate = new List<ProductVariant>();

        foreach (var req in requests)
        {
            var variant = await productRepository.GetVariantByIdAsync(req.VariantId, ct);
            if (variant is null)
                return Result.Failure(Error.NotFound("ProductVariant", req.VariantId));

            if (!variant.TryReserveStock(req.Quantity))
                return Result.Failure(Error.Validation(
                    "Stock.Insufficient",
                    $"Insufficient available stock for variant {req.VariantId}. Requested: {req.Quantity}, Available: {variant.AvailableQuantity}"));

            variantsToUpdate.Add(variant);
        }

        foreach (var variant in variantsToUpdate)
        {
            productRepository.UpdateVariantAsync(variant);
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> ReleaseStocksAsync(IEnumerable<StockDeductionRequest> requests, CancellationToken ct = default)
    {
        foreach (var req in requests)
        {
            var variant = await productRepository.GetVariantByIdAsync(req.VariantId, ct);
            if (variant is not null)
            {
                variant.ReleaseReservedStock(req.Quantity);
                productRepository.UpdateVariantAsync(variant);
            }
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }

    public async Task<Result> CommitStocksAsync(IEnumerable<StockDeductionRequest> requests, CancellationToken ct = default)
    {
        var variantsToUpdate = new List<ProductVariant>();

        foreach (var req in requests)
        {
            var variant = await productRepository.GetVariantByIdAsync(req.VariantId, ct);
            if (variant is null)
                return Result.Failure(Error.NotFound("ProductVariant", req.VariantId));

            if (!variant.TryCommitReservedStock(req.Quantity))
                return Result.Failure(Error.Validation(
                    "Stock.Insufficient",
                    $"Insufficient reserved/total stock for variant {req.VariantId}. Requested: {req.Quantity}"));

            variantsToUpdate.Add(variant);
        }

        foreach (var variant in variantsToUpdate)
        {
            productRepository.UpdateVariantAsync(variant);
        }

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success();
    }
}
