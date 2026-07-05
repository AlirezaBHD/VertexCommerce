using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Catalog.Domain.Products;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TResult?> GetByIdAsync<TResult>(Guid id, ISpecification<Product, TResult> spec,
        CancellationToken ct = default);
    Task<IReadOnlyList<TResult>> GetAttributes<TResult>(ISpecification<CatalogAttribute, TResult> spec,
        CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    Task AddVariantAsync(ProductVariant variant, CancellationToken ct = default);
    void Delete(Product product);
    Task<bool> HasProductsInCategoryAsync(Guid categoryId, CancellationToken ct = default);
    Task<Product?> GetByIdWithVariantsAsync(Guid id, CancellationToken ct);
    Task<bool> SlugExistsAsync(string slug, CancellationToken ct);
    void UpdateVariantAsync(ProductVariant variant);
    Task<ProductVariant?> GetVariantByIdAsync(Guid variantId, CancellationToken ct = default);
}
