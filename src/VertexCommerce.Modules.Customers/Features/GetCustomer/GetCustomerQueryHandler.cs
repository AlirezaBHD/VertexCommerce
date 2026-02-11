using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.GetCustomer;

internal sealed class GetCustomerQueryHandler : IQueryHandler<GetCustomerQuery, CustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerQueryHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<Result<CustomerResponse>> Handle(GetCustomerQuery query, CancellationToken ct)
    {
        var customer = await _customerRepository.GetByUserIdAsync(query.UserId, ct);

        if (customer is null)
            return Result.Failure<CustomerResponse>(Error.NotFound("Customer", query.UserId));

        return Result.Success(new CustomerResponse(
            customer.Id,
            customer.UserId,
            customer.Email,
            customer.FirstName,
            customer.LastName,
            customer.FullName,
            customer.Phone,
            customer.Addresses.Select(a => new AddressResponse(
                a.Id,
                a.Street,
                a.City,
                a.State,
                a.Country,
                a.ZipCode,
                a.Label,
                a.FullAddress
            )).ToList(),
            customer.DefaultShippingAddressId,
            customer.DefaultBillingAddressId
        ));
    }
}
