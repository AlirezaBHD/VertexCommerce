using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace VertexCommerce.Modules.Catalog.ReadModels;

internal sealed class ProductReadModelRepository : IProductReadModelRepository
{
    private readonly IMongoCollection<ProductReadModel> _collection;

    public ProductReadModelRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<ProductReadModel>("products");
        CreateIndexes();//TODO
        //Change it. it's dangerous
    }

    private void CreateIndexes()
    {
        var indexKeys = Builders<ProductReadModel>.IndexKeys;

        _collection.Indexes.CreateMany([
            new CreateIndexModel<ProductReadModel>(indexKeys.Ascending(p => p.Sku), new() { Unique = true }),
            new CreateIndexModel<ProductReadModel>(indexKeys.Ascending(p => p.CategoryId)),
            new CreateIndexModel<ProductReadModel>(indexKeys.Ascending(p => p.IsActive)),
            new CreateIndexModel<ProductReadModel>(indexKeys.Descending(p => p.CreatedAt)),
            new CreateIndexModel<ProductReadModel>(indexKeys.Ascending(p => p.Price)),
            new CreateIndexModel<ProductReadModel>(indexKeys.Text(p => p.SearchText))
        ]);
    }

    public async Task<ProductReadModel?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _collection.Find(p => p.Id == id).FirstOrDefaultAsync(ct);
    }

    public async Task<ProductReadModel?> GetBySkuAsync(string sku, CancellationToken ct = default)
    {
        return await _collection.Find(p => p.Sku == sku).FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<ProductReadModel>> GetFeaturedAsync(int take, CancellationToken ct = default)
    {
        return await _collection
            .Find(p => p.IsActive && p.StockQuantity > 0)
            .SortByDescending(p => p.CreatedAt)
            .Limit(take)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ProductReadModel>> GetNewAsync(int take, CancellationToken ct = default)
    {
        return await _collection
            .Find(p => p.IsActive)
            .SortByDescending(p => p.CreatedAt)
            .Limit(take)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ProductReadModel>> GetByCategoryAsync(
        Guid categoryId,
        int skip,
        int take,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStock,
        string? sortBy,
        bool descending,
        CancellationToken ct = default)
    {
        var filter = BuildCategoryFilter(categoryId, minPrice, maxPrice, inStock);
        var sort = BuildSort(sortBy, descending);

        return await _collection
            .Find(filter)
            .Sort(sort)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<ProductReadModel>> SearchAsync(
        string searchTerm,
        int skip,
        int take,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        CancellationToken ct = default)
    {
        var filter = BuildSearchFilter(searchTerm, categoryId, minPrice, maxPrice);

        return await _collection
            .Find(filter)
            .SortByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Limit(take)
            .ToListAsync(ct);
    }

    public async Task<long> CountByCategoryAsync(Guid categoryId, CancellationToken ct = default)
    {
        return await _collection.CountDocumentsAsync(
            p => p.CategoryId == categoryId && p.IsActive, 
            cancellationToken: ct);
    }

    public async Task<long> CountSearchAsync(string searchTerm, Guid? categoryId, CancellationToken ct = default)
    {
        var filter = BuildSearchFilter(searchTerm, categoryId, null, null);
        return await _collection.CountDocumentsAsync(filter, cancellationToken: ct);
    }

    public async Task UpsertAsync(ProductReadModel model, CancellationToken ct = default)
    {
        model.SyncedAt = DateTime.UtcNow;

        await _collection.ReplaceOneAsync(
            p => p.Id == model.Id,
            model,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await _collection.DeleteOneAsync(p => p.Id == id, ct);
    }

    public async Task UpsertManyAsync(IEnumerable<ProductReadModel> models, CancellationToken ct = default)
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
        {
            await _collection.BulkWriteAsync(writes, cancellationToken: ct);
        }
    }

    #region Filter Builders

    private static FilterDefinition<ProductReadModel> BuildCategoryFilter(
        Guid categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        bool? inStock)
    {
        var builder = Builders<ProductReadModel>.Filter;
        var filter = builder.Eq(p => p.CategoryId, categoryId) & builder.Eq(p => p.IsActive, true);

        if (minPrice.HasValue)
            filter &= builder.Gte(p => p.Price, minPrice.Value);

        if (maxPrice.HasValue)
            filter &= builder.Lte(p => p.Price, maxPrice.Value);

        if (inStock == true)
            filter &= builder.Gt(p => p.StockQuantity, 0);

        return filter;
    }

    private static FilterDefinition<ProductReadModel> BuildSearchFilter(
        string searchTerm,
        Guid? categoryId,
        decimal? minPrice,
        decimal? maxPrice)
    {
        var builder = Builders<ProductReadModel>.Filter;
        var term = searchTerm.ToLower();

        // Text search or contains
        var filter = builder.Eq(p => p.IsActive, true) &
            (builder.Text(searchTerm) |
             builder.Regex(p => p.Name, new MongoDB.Bson.BsonRegularExpression(term, "i")) |
             builder.Regex(p => p.Sku, new MongoDB.Bson.BsonRegularExpression(term, "i")));

        if (categoryId.HasValue)
            filter &= builder.Eq(p => p.CategoryId, categoryId.Value);

        if (minPrice.HasValue)
            filter &= builder.Gte(p => p.Price, minPrice.Value);

        if (maxPrice.HasValue)
            filter &= builder.Lte(p => p.Price, maxPrice.Value);

        return filter;
    }

    private static SortDefinition<ProductReadModel> BuildSort(string? sortBy, bool descending)
    {
        var builder = Builders<ProductReadModel>.Sort;

        return sortBy?.ToLower() switch
        {
            "price" => descending ? builder.Descending(p => p.Price) : builder.Ascending(p => p.Price),
            "name" => descending ? builder.Descending(p => p.Name) : builder.Ascending(p => p.Name),
            "stock" => descending ? builder.Descending(p => p.StockQuantity) : builder.Ascending(p => p.StockQuantity),
            _ => builder.Descending(p => p.CreatedAt)
        };
    }

    #endregion
}