using MongoDB.Driver;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;

internal sealed class BannerIndexManager(IMongoDatabase database)
{
    private readonly IMongoCollection<BannerDocument> _collection = database.GetCollection<BannerDocument>("banners");

    public async Task EnsureIndexesAsync(CancellationToken ct = default)
    {
        var indexKeys = Builders<BannerDocument>.IndexKeys;

        var indexes = new List<CreateIndexModel<BannerDocument>>
        {
            new(
                indexKeys.Ascending(b => b.IsActive).Ascending(b => b.SortOrder),
                new CreateIndexOptions { Name = "idx_banner_active_sort" }
            )
        };

        await _collection.Indexes.CreateManyAsync(indexes, ct);
    }
}
