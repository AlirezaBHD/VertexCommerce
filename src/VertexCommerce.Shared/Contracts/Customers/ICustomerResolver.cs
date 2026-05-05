namespace VertexCommerce.Shared.Contracts.Customers;

public interface ICustomerResolver
{
    Task<Guid> GetCustomerIdByUserIdAsync(Guid userId, CancellationToken ct = default);
}