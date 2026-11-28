using VertexCommerce.Shared.Contracts.Customers;

namespace VertexCommerce.Modules.Basket.Features;

internal static class CustomerResolverExtensions
{
    public static async Task<Guid?> ResolveCustomerIdAsync(
        this ICustomerResolver customerResolver,
        Guid userId,
        CancellationToken ct)
    {
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(userId, ct);

        return customerId == Guid.Empty ? null : customerId;
    }
}
