using HotChocolate;
using HotChocolate.Data;
using MongoDB.Driver;
using VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProducts;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Products;

internal sealed class ProductReadModelRepository : IProductReadModelRepository
{
    private readonly IMongoCollection<ProductReadModel> _collection;

    public ProductReadModelRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<ProductReadModel>("products");
    }

    public IMongoCollection<ProductReadModel> GetCollection() => _collection;

    public IExecutable<ProductReadModel> GetProducts(CancellationToken ct = default)
    {
        return _collection
            .Find(p => p.IsActive)
            .AsExecutable();
    }

    public IExecutable<ProductReadModel> GetFilteredProducts(
        string? searchTerm = null,
        Guid? categoryId = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        bool? isActive = null)
    {
        var filter = ProductQueryFilterBuilder.BuildSearchFilter(
            searchTerm,categoryId, minPrice, maxPrice , isActive);

        return _collection
            .Find(filter)
            .AsExecutable();
    }

    
    public IExecutable<ProductReadModel> GetByIdAsync(
        Guid id,
        CancellationToken ct = default)
    {
        return _collection
            .Find(p => p.Id == id && p.IsActive)
            .AsExecutable();
    }

    public IExecutable<ProductReadModel> GetProductsByCategory(
        Guid categoryId,
        bool? inStock,
        decimal? minPrice,
        decimal? maxPrice)
    {
        var filter = ProductQueryFilterBuilder.BuildCategoryFilter(
            categoryId, minPrice, maxPrice, inStock);

        return _collection
            .Find(filter)
            .AsExecutable();
    }

    public async Task<(IReadOnlyList<ProductReadModel> Items, long TotalCount)>
        GetByCategoryAsync(
            Guid categoryId,
            int skip,
            int take,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            bool? inStock = null,
            string? sortBy = null,
            bool descending = true,
            CancellationToken ct = default)
    {
        var filter = ProductQueryFilterBuilder.BuildCategoryFilter(
            categoryId, minPrice, maxPrice, inStock);
        var sort = ProductQueryFilterBuilder.BuildSort(sortBy, descending);

        var itemsTask = _collection
            .Find(filter)
            .Sort(sort)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(ct);

        var countTask = _collection
            .CountDocumentsAsync(filter, cancellationToken: ct);

        await Task.WhenAll(itemsTask, countTask);

        return (itemsTask.Result, countTask.Result);
    }

    public async Task<(IReadOnlyList<ProductReadModel> Items, long TotalCount)>
        SearchAsync(
            string searchTerm,
            int skip,
            int take,
            Guid? categoryId = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            CancellationToken ct = default)
    {
        var filter = ProductQueryFilterBuilder.BuildSearchFilter(
            searchTerm, categoryId, minPrice, maxPrice);

        var itemsTask = _collection
            .Find(filter)
            .SortByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(ct);

        var countTask = _collection
            .CountDocumentsAsync(filter, cancellationToken: ct);

        await Task.WhenAll(itemsTask, countTask);

        return (itemsTask.Result, countTask.Result);
    }

    public async Task UpsertAsync(
        ProductReadModel model,
        CancellationToken ct = default)
    {
        model.SyncedAt = DateTime.UtcNow;

        await _collection.ReplaceOneAsync(
            Builders<ProductReadModel>.Filter.Eq(p => p.Id, model.Id),
            model,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }

    public async Task UpsertManyAsync(
        IEnumerable<ProductReadModel> models,
        CancellationToken ct = default)
    {
        var writes = models.Select(model =>
        {
            model.SyncedAt = DateTime.UtcNow;
            return new ReplaceOneModel<ProductReadModel>(
                Builders<ProductReadModel>.Filter.Eq(p => p.Id, model.Id),
                model)
            { IsUpsert = true };
        }).ToList();

        if (writes.Count > 0)
            await _collection.BulkWriteAsync(writes, cancellationToken: ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _collection.DeleteOneAsync(
            Builders<ProductReadModel>.Filter.Eq(p => p.Id, id), ct);
    }
}
