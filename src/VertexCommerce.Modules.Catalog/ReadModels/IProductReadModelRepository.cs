namespace VertexCommerce.Modules.Catalog.ReadModels;

public interface IProductReadModelRepository
{
    // Single
    Task<ProductReadModel?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProductReadModel?> GetBySkuAsync(string sku, CancellationToken ct = default);

    // Lists
    Task<IReadOnlyList<ProductReadModel>> GetFeaturedAsync(int take, CancellationToken ct = default);
    Task<IReadOnlyList<ProductReadModel>> GetNewAsync(int take, CancellationToken ct = default);
    Task<IReadOnlyList<ProductReadModel>> GetByCategoryAsync(
        Guid categoryId,
        int skip,
        int take,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStock,
        string? sortBy,
        bool descending,
        CancellationToken ct = default);

    // Search
    Task<IReadOnlyList<ProductReadModel>> SearchAsync(
        string searchTerm,
        int skip,
        int take,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        CancellationToken ct = default);

    // Count
    Task<long> CountByCategoryAsync(Guid categoryId, CancellationToken ct = default);
    Task<long> CountSearchAsync(string searchTerm, Guid? categoryId, CancellationToken ct = default);

    // Sync
    Task UpsertAsync(ProductReadModel model, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task UpsertManyAsync(IEnumerable<ProductReadModel> models, CancellationToken ct = default);
}
