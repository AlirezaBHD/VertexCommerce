using HotChocolate;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;


public interface ICategoryReadModelRepository
{
    IExecutable<CategoryReadModel> GetByIdAsync(Guid id, CancellationToken ct = default);
    IExecutable<CategoryReadModel> GetBySlugAsync(string slug, CancellationToken ct = default);
    IExecutable<CategoryReadModel> GetAllAsync(bool? isActive = null, CancellationToken ct = default);
    Task UpsertAsync(CategoryReadModel model, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
    IExecutable<CategoryReadModel> GetFilteredCategories(bool? isActive, bool? showOnHome, bool? showOnMenu);
}
