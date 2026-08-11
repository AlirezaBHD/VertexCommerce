using HotChocolate;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;
using VertexCommerce.Shared.Contracts.Catalog;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Products;

public interface IProductReadModelRepository
{
    IExecutable<ProductReadModel> GetFilteredProducts(
        string? searchTerm = null,
        Guid? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? isActive = null);
    
    Task<ProductVariantInfo?> GetProductVariantInfoAsync(Guid productId, Guid variantId, CancellationToken ct = default);
    Task UpsertAsync(ProductReadModel model, CancellationToken ct = default);
    Task UpsertManyAsync(IEnumerable<ProductReadModel> models, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    IExecutable<ProductReadModel> GetLatestProducts(int limit);
    IExecutable<ProductReadModel> GetBySlugAsync(string slug);
    IExecutable<ProductReadModel> GetAll();
    Task<IReadOnlyList<ProductReadModel>> SearchAsync(string? searchTerm, int limit, CancellationToken ct = default);
}
