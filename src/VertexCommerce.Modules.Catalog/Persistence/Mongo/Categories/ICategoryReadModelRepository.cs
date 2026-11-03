using HotChocolate;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;


public interface ICategoryReadModelRepository
{
    IExecutable<CategoryReadModel> GetById(Guid id, CancellationToken ct = default);
    IExecutable<CategoryReadModel> GetBySlug(string slug, CancellationToken ct = default);
    IExecutable<CategoryReadModel> GetAll(bool? isActive = null, CancellationToken ct = default);
    Task UpsertAsync(CategoryReadModel model, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    IExecutable<CategoryReadModel> GetFilteredCategories(bool? isActive, bool? showOnHome, bool? showOnMenu);
    Task<IReadOnlyList<CategoryReadModel>> SearchAsync(string? searchTerm, int limit, CancellationToken ct = default);
}
