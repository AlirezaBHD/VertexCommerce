using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Queries.GetAddressById;

internal sealed class GetAddressByIdQueryHandler(
    ICustomerAddressRepository addressRepository,
    ICurrentUser currentUser,
    ICustomerResolver  customerResolver)
    : IQueryHandler<GetAddressByIdQuery, AddressResponse>
{
    public async Task<Result<AddressResponse>> Handle(GetAddressByIdQuery query, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(userId, ct);

        if (customerId == Guid.Empty)
        {
            return Result.Failure<AddressResponse>(Error.NotFound("Customer", userId));
        }
        
        var spec = new GetAddressByIdSpec(addressId: query.AddressId,  customerId: customerId);
        
        var address = await addressRepository.GetAsync
            (spec:spec, ct);

        if (address is null)
        {
            return Result.Failure<AddressResponse>(Error.NotFound("Address", query.AddressId));

        }
        
        return Result.Success(address);
    }
}
