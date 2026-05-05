using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VertexCommerce.Modules.Basket.Contract;
using VertexCommerce.Modules.Basket.Persistence.Configuration;
using VertexCommerce.Modules.Basket.Persistence.Documents;

namespace VertexCommerce.Modules.Basket.Persistence;

internal sealed class BasketRepository : IBasketRepository
{
    private readonly IMongoCollection<BasketDocument> _baskets;

    public BasketRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _baskets = database.GetCollection<BasketDocument>(settings.Value.BasketsCollectionName);

        CreateIndexes();
    }

    private void CreateIndexes()
    {
        // ─── CustomerId unique index ───
        var customerIdIndex = Builders<BasketDocument>.IndexKeys
            .Ascending(b => b.CustomerId);

        _baskets.Indexes.CreateOne(new CreateIndexModel<BasketDocument>(
            customerIdIndex,
            new CreateIndexOptions { Unique = true }));

        // ─── TTL index ───
        var ttlIndex = Builders<BasketDocument>.IndexKeys
            .Ascending(b => b.ExpiresAt);

        _baskets.Indexes.CreateOne(new CreateIndexModel<BasketDocument>(
            ttlIndex,
            new CreateIndexOptions { ExpireAfter = TimeSpan.Zero }));
    }

    public async Task<BasketDocument?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _baskets
            .Find(b => b.CustomerId == customerId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task UpsertAsync(BasketDocument basket, CancellationToken ct = default)
    {
        await _baskets.ReplaceOneAsync(
            b => b.CustomerId == basket.CustomerId,
            basket,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }

    public async Task DeleteAsync(Guid customerId, CancellationToken ct = default)
    {
        await _baskets.DeleteOneAsync(
            b => b.CustomerId == customerId,
            ct);
    }
}
