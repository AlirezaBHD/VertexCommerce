namespace VertexCommerce.Modules.Catalog.Sync.Products;

public interface IProductSyncService
{
    /// <summary>
    /// Sync a single product by its Domain Entity (when you already have it loaded)
    /// </summary>
    Task SyncProductAsync(Guid productId, CancellationToken ct = default);

    /// <summary>
    /// Delete product from read store
    /// </summary>
    Task DeleteProductAsync(Guid productId, CancellationToken ct = default);
    
    Task SyncAllProductsAsync(CancellationToken ct = default);
    
    Task SyncCategoryProductsAsync(Guid categoryId, CancellationToken ct = default);
}
