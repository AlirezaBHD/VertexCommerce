using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Shared.Contracts.Customers;

namespace VertexCommerce.Modules.Customers.Infrastructure.Services;

internal class CustomerResolver(ICustomerRepository repository) : ICustomerResolver
{
    public async Task<Guid> GetCustomerIdByUserIdAsync(Guid userId, CancellationToken ct)
    {
        var customerId = await repository.GetIdByUserIdAsync(userId, ct);

        if (customerId == Guid.Empty)
            throw new Exception($"Customer not found for userId: {userId}");

        return customerId;
    }
}
