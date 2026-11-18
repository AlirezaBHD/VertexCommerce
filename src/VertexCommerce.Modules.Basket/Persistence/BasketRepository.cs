using Microsoft.Extensions.Options;
using MongoDB.Driver;
using VertexCommerce.Modules.Basket.Contract;
using VertexCommerce.Modules.Basket.Persistence.Configuration;
using VertexCommerce.Modules.Basket.Persistence.Documents;

namespace VertexCommerce.Modules.Basket.Persistence;

/// <summary>
/// MongoDB-backed basket store. The <see cref="IMongoDatabase"/> singleton is provided
/// by the host; this repository owns no client and creates no connections itself.
/// </summary>
internal sealed class BasketRepository(
    IMongoDatabase database,
    IOptions<MongoDbSettings> settings) : IBasketRepository
{
    private readonly IMongoCollection<BasketDocument> _baskets =
        database.GetCollection<BasketDocument>(settings.Value.BasketsCollectionName);

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
