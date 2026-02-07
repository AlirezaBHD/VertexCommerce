using VertexCommerce.Modules.Catalog.Domain.Entities;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Catalog.Domain.Repositories;

public interface ICategoryRepository : IRepository<Category, Guid>
{
    Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Category>> GetChildrenAsync(Guid parentId, CancellationToken ct = default);
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
    Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default);
}
