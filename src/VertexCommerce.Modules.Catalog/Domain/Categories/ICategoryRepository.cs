using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Catalog.Domain.Categories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, Guid? excludeId, CancellationToken ct = default);
    Task AddAsync(Category category, CancellationToken ct = default);
    void Update(Category category);
    Task<IReadOnlyList<TResult>> ListAsync<TResult>(
        ISpecification<Category, TResult> spec,
        CancellationToken ct = default);

    Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string slug, Guid? excludeId, CancellationToken ct);
}
