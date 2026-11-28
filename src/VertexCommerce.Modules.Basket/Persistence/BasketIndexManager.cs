using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VertexCommerce.Modules.Basket.Persistence.Configuration;
using VertexCommerce.Modules.Basket.Persistence.Documents;

namespace VertexCommerce.Modules.Basket.Persistence;

internal sealed class BasketIndexManager(
    IMongoDatabase database,
    IOptions<MongoDbSettings> settings)
{
    private readonly IMongoCollection<BasketDocument> _baskets =
        database.GetCollection<BasketDocument>(settings.Value.BasketsCollectionName);

    public async Task EnsureIndexesAsync(CancellationToken ct = default)
    {
        var indexKeys = Builders<BasketDocument>.IndexKeys;

        var indexes = new List<CreateIndexModel<BasketDocument>>
        {
            // A customer has at most one basket; upserts are keyed by CustomerId.
            new(
                indexKeys.Ascending(b => b.CustomerId),
                new CreateIndexOptions { Unique = true, Name = "idx_customer_id_unique" }
            ),
            new(
                indexKeys.Ascending(b => b.ExpiresAt),
                new CreateIndexOptions { ExpireAfter = TimeSpan.Zero, Name = "idx_expires_at_ttl" }
            )
        };

        await _baskets.Indexes.CreateManyAsync(indexes, ct);
    }
}
