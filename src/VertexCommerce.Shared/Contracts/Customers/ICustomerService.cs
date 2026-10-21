namespace VertexCommerce.Shared.Contracts.Customers;

public interface ICustomerService
{
    Task<CustomerInfoDto?> GetCustomerInfo(Guid customerId,
        CancellationToken ct = default);
}
