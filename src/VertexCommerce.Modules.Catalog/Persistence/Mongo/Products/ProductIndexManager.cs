using MongoDB.Driver;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Products.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Products;

internal sealed class ProductIndexManager(IMongoDatabase database)
{
    private readonly IMongoCollection<ProductReadModel> _collection = database.GetCollection<ProductReadModel>("products");

    public async Task EnsureIndexesAsync(CancellationToken ct = default)
    {
        var indexKeys = Builders<ProductReadModel>.IndexKeys;

        var indexes = new List<CreateIndexModel<ProductReadModel>>
        {
            new(
                indexKeys.Ascending(p => p.CategoryId).Ascending(p => p.IsActive),
                new CreateIndexOptions { Name = "idx_category_active" }
            ),
            new(
                indexKeys.Descending(p => p.CreatedAt),
                new CreateIndexOptions { Name = "idx_created_desc" }
            ),
            new(
                indexKeys.Ascending(p => p.MinPrice),
                new CreateIndexOptions { Name = "idx_min_price" }
            ),
            new(
                indexKeys.Ascending(p => p.MaxPrice),
                new CreateIndexOptions { Name = "idx_max_price" }
            ),
            new(
                indexKeys.Text(p => p.SearchText),
                new CreateIndexOptions { Name = "idx_search_text" }
            ),
            new(
                indexKeys
                    .Ascending(p => p.CategoryId)
                    .Ascending(p => p.IsActive)
                    .Ascending(p => p.MinPrice),
                new CreateIndexOptions { Name = "idx_category_active_price" }
            )
        };

        await _collection.Indexes.CreateManyAsync(indexes, ct);
    }
}
