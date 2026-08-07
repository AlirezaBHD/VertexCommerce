using HotChocolate;
using HotChocolate.Data;
using MongoDB.Driver;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;

internal sealed class CategoryReadModelRepository(
    IMongoDatabase database,
    CategoryIndexManager indexManager)
    : ICategoryReadModelRepository
{
    private readonly IMongoCollection<CategoryReadModel> _collection = database.GetCollection<CategoryReadModel>("categories");
    private readonly CategoryIndexManager _indexManager = indexManager;

    public IExecutable<CategoryReadModel> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return  _collection
            .Find(c => c.Id == id)
            .AsExecutable();
    }

    public IExecutable<CategoryReadModel> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        return _collection
            .Find(c => c.Slug == slug && c.IsActive)
            .AsExecutable();
    }

    public IExecutable<CategoryReadModel> GetAllAsync(
        bool? isActive = null, CancellationToken ct = default)
    {
        var filter = CategoryQueryFilterBuilder.BuildListFilter(isActive);
        var sort = CategoryQueryFilterBuilder.BuildDefaultSort();

        return  _collection
            .Find(filter)
            .Sort(sort).AsExecutable();
    }

    public async Task UpsertAsync(
        CategoryReadModel model, CancellationToken ct = default)
    {
        var filter = Builders<CategoryReadModel>.Filter.Eq(c => c.Id, model.Id);

        await _collection.ReplaceOneAsync(
            filter,
            model,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _collection.DeleteOneAsync(c => c.Id == id, ct);
    }

    public IExecutable<CategoryReadModel> GetFilteredCategories(bool? isActive, bool? showOnHome, bool? showOnMenu)
    {
        var filter = CategoryQueryFilterBuilder.BuildListFilter(isActive);
        var sort = CategoryQueryFilterBuilder.BuildDefaultSort();

        return _collection
            .Find(filter)
            .Sort(sort)
            .AsExecutable();
    }
}
