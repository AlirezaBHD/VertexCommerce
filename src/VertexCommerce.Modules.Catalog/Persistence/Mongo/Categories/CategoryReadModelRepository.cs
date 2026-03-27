using HotChocolate;
using HotChocolate.Data;
using MongoDB.Driver;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;

internal sealed class CategoryReadModelRepository : ICategoryReadModelRepository
{
    private readonly IMongoCollection<CategoryReadModel> _collection;
    private readonly CategoryIndexManager _indexManager;

    public CategoryReadModelRepository(
        IMongoDatabase database,
        CategoryIndexManager indexManager)
    {
        _collection = database.GetCollection<CategoryReadModel>("categories");
        _indexManager = indexManager;
    }

    public IExecutable<CategoryReadModel> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return  _collection
            .Find(c => c.Id == id)
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

    public async Task<List<CategoryReadModel>> GetRootCategoriesAsync(
        CancellationToken ct = default)
    {
        var filter = CategoryQueryFilterBuilder.BuildRootFilter();
        var sort = CategoryQueryFilterBuilder.BuildDefaultSort();

        return await _collection
            .Find(filter)
            .Sort(sort)
            .ToListAsync(ct);
    }

    public async Task<List<CategoryReadModel>> GetChildrenAsync(
        Guid parentId, CancellationToken ct = default)
    {
        var filter = CategoryQueryFilterBuilder.BuildChildrenFilter(parentId);
        var sort = CategoryQueryFilterBuilder.BuildDefaultSort();

        return await _collection
            .Find(filter)
            .Sort(sort)
            .ToListAsync(ct);
    }

    public async Task<List<CategoryReadModel>> GetByIdsAsync(
        IEnumerable<Guid> ids, CancellationToken ct = default)
    {
        var filter = Builders<CategoryReadModel>.Filter
            .In(c => c.Id, ids);

        return await _collection
            .Find(filter)
            .ToListAsync(ct);
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

    public async Task UpdateProductCountAsync(
        Guid categoryId, int count, CancellationToken ct = default)
    {
        var filter = Builders<CategoryReadModel>.Filter.Eq(c => c.Id, categoryId);
        var update = Builders<CategoryReadModel>.Update
            .Set(c => c.ProductCount, count)
            .Set(c => c.UpdatedAt, DateTime.UtcNow);

        await _collection.UpdateOneAsync(filter, update, cancellationToken: ct);
    }

    public async Task EnsureIndexesAsync(CancellationToken ct = default)
    {
        await _indexManager.EnsureIndexesAsync(ct);
    }
}
