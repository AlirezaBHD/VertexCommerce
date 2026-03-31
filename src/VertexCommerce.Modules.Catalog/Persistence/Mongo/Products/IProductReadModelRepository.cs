using HotChocolate;
using MongoDB.Driver;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Products;

public interface IProductReadModelRepository
{
    IMongoCollection<ProductReadModel> GetCollection();
    IExecutable<ProductReadModel> GetProducts(CancellationToken ct = default);
    IExecutable<ProductReadModel> GetByIdAsync(Guid id, CancellationToken ct = default);

    IExecutable<ProductReadModel> GetFilteredProducts(
        string? searchTerm = null,
        Guid? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? isActive = null);
    IExecutable<ProductReadModel> GetProductsByCategory(
        Guid categoryId, bool? inStock, decimal? minPrice, decimal? maxPrice);

    // ── Paginated queries ──
    Task<(IReadOnlyList<ProductReadModel> Items, long TotalCount)> GetByCategoryAsync(
        Guid categoryId, int skip, int take,
        decimal? minPrice = null, decimal? maxPrice = null,
        bool? inStock = null, string? sortBy = null, bool descending = true,
        CancellationToken ct = default);

    Task<(IReadOnlyList<ProductReadModel> Items, long TotalCount)> SearchAsync(
        string searchTerm, int skip, int take,
        Guid? categoryId = null, decimal? minPrice = null, decimal? maxPrice = null,
        CancellationToken ct = default);

    // ── Write (Sync from PostgreSQL) ──
    Task UpsertAsync(ProductReadModel model, CancellationToken ct = default);
    Task UpsertManyAsync(IEnumerable<ProductReadModel> models, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
