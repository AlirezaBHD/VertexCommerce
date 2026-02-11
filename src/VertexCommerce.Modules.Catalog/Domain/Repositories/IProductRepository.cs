using VertexCommerce.Modules.Catalog.Domain.Entities;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Catalog.Domain.Repositories;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);
    Task<bool> SkuExistsAsync(string sku, CancellationToken ct = default);
    Task AddAsync(Product product, CancellationToken ct = default);
    void Update(Product product);
    void Delete(Product product);
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(
        ISpecification<Product, TResult> spec,
        CancellationToken ct = default);
    Task<int> CountAsync(
        ISpecification<Product> spec,
        CancellationToken ct = default);
    Task<bool> HasProductsInCategoryAsync(Guid categoryId, CancellationToken ct = default);
}
