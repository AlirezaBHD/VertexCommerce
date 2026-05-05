namespace VertexCommerce.Modules.Catalog.Sync.Products;

public interface IProductSyncService
{
    Task SyncProductAsync(Guid productId, CancellationToken ct = default);

    Task DeleteProductAsync(Guid productId, CancellationToken ct = default);
    
    Task SyncAllProductsAsync(CancellationToken ct = default);
    
    Task SyncCategoryProductsAsync(Guid categoryId, CancellationToken ct = default);
}
