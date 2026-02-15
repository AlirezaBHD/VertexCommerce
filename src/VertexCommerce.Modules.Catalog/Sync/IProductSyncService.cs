using VertexCommerce.Modules.Catalog.Domain.Entities;

namespace VertexCommerce.Modules.Catalog.Sync;

public interface IProductSyncService
{
    Task SyncProductAsync(Product product, CancellationToken ct = default);
    Task SyncProductAsync(Guid productId, CancellationToken ct = default);
    Task DeleteProductAsync(Guid productId, CancellationToken ct = default);
    Task SyncAllProductsAsync(CancellationToken ct = default);
    Task SyncCategoryProductsAsync(Guid categoryId, CancellationToken ct = default);
}
