using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VertexCommerce.Modules.Basket.Domain.Entities;
using VertexCommerce.Modules.Basket.Domain.Repositories;

namespace VertexCommerce.Modules.Basket.Persistence;

public sealed class BasketRepository : IBasketRepository
{
    private readonly IMongoCollection<CustomerBasket> _baskets;

    public BasketRepository(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _baskets = database.GetCollection<CustomerBasket>(settings.Value.BasketsCollectionName);

        // Create indexes
        CreateIndexes();
    }

    private void CreateIndexes()
    {
        var customerIdIndex = new CreateIndexModel<CustomerBasket>(
            Builders<CustomerBasket>.IndexKeys.Ascending(b => b.CustomerId),
            new CreateIndexOptions { Unique = true });

        var expiresAtIndex = new CreateIndexModel<CustomerBasket>(
            Builders<CustomerBasket>.IndexKeys.Ascending(b => b.ExpiresAt),
            new CreateIndexOptions { ExpireAfter = TimeSpan.Zero });

        _baskets.Indexes.CreateMany([customerIdIndex, expiresAtIndex]);
    }

    public async Task<CustomerBasket?> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _baskets
            .Find(b => b.CustomerId == customerId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<CustomerBasket?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _baskets
            .Find(b => b.Id == id)
            .FirstOrDefaultAsync(ct);
    }

    public async Task CreateAsync(CustomerBasket basket, CancellationToken ct = default)
    {
        await _baskets.InsertOneAsync(basket, cancellationToken: ct);
    }

    public async Task UpdateAsync(CustomerBasket basket, CancellationToken ct = default)
    {
        await _baskets.ReplaceOneAsync(
            b => b.CustomerId == basket.CustomerId,
            basket,
            new ReplaceOptions { IsUpsert = true },
            ct);
    }

    public async Task DeleteAsync(Guid customerId, CancellationToken ct = default)
    {
        await _baskets.DeleteOneAsync(b => b.CustomerId == customerId, ct);
    }

    public async Task<bool> ExistsAsync(Guid customerId, CancellationToken ct = default)
    {
        return await _baskets
            .Find(b => b.CustomerId == customerId)
            .AnyAsync(ct);
    }
}