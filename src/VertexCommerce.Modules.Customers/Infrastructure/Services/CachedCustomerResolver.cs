using Microsoft.Extensions.Caching.Memory;
using VertexCommerce.Shared.Contracts.Customers;

namespace VertexCommerce.Modules.Customers.Infrastructure.Services;

internal class CachedCustomerResolver(ICustomerResolver inner, IMemoryCache cache) : ICustomerResolver
{
    public async Task<Guid> GetCustomerIdByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var key = $"customer:user:{userId}";

        return await cache.GetOrCreateAsync(key, async entry =>
        {
            entry.SlidingExpiration = TimeSpan.FromMinutes(30);
            return await inner.GetCustomerIdByUserIdAsync(userId, ct);
        });
    }
}
