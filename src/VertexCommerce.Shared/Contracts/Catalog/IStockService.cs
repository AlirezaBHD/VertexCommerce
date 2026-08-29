using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Shared.Contracts.Catalog;

public interface IStockService
{
    Task<Result> DeductStockAsync(Guid variantId, int quantity, CancellationToken ct = default);
    Task<Result> DeductStocksAsync(IEnumerable<StockDeductionRequest> requests, CancellationToken ct = default);
    Task<Result> ReserveStocksAsync(IEnumerable<StockDeductionRequest> requests, CancellationToken ct = default);
    Task<Result> ReleaseStocksAsync(IEnumerable<StockDeductionRequest> requests, CancellationToken ct = default);
    Task<Result> CommitStocksAsync(IEnumerable<StockDeductionRequest> requests, CancellationToken ct = default);
}

public record StockDeductionRequest(Guid VariantId, int Quantity);
