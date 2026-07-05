using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Shared.Contracts.Catalog;

public interface IStockService
{
    Task<Result> DeductStockAsync(Guid variantId, int quantity, CancellationToken ct = default);
}
