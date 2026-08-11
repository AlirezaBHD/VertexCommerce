namespace VertexCommerce.Modules.Catalog.Sync.Categories;

public interface ICategorySyncService
{
    Task SyncCategoryAsync(Guid categoryId, CancellationToken ct = default);
    Task SyncAllCategoriesAsync(CancellationToken ct = default);
    Task DeleteCategoryAsync(Guid categoryId, CancellationToken ct = default);
}
