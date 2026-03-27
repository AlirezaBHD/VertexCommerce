using HotChocolate;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;


public interface ICategoryReadModelRepository
{
    IExecutable<CategoryReadModel> GetByIdAsync(Guid id, CancellationToken ct = default);
    IExecutable<CategoryReadModel> GetAllAsync(bool? isActive = null, CancellationToken ct = default);
    Task<List<CategoryReadModel>> GetRootCategoriesAsync(CancellationToken ct = default);
    Task<List<CategoryReadModel>> GetChildrenAsync(Guid parentId, CancellationToken ct = default);
    Task<List<CategoryReadModel>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
    Task UpsertAsync(CategoryReadModel model, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    Task UpdateProductCountAsync(Guid categoryId, int count, CancellationToken ct = default);
    Task EnsureIndexesAsync(CancellationToken ct = default);
}
