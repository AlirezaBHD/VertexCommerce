using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomer;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AddAddress;

internal sealed class AddAddressCommandHandler(
    ICustomerRepository customerRepository,
    ICustomerAddressRepository addressRepository,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver,
    ICustomerUnitOfWork unitOfWork)
    : ICommandHandler<AddAddressCommand, AddressSummaryResponse>
{
    public async Task<Result<AddressSummaryResponse>> Handle(AddAddressCommand command, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(userId, ct);
        var customer = await customerRepository.GetByIdAsync(customerId, ct);

        if (customer is null)
        {
            return Result.Failure<AddressSummaryResponse>(Error.NotFound("Customer", userId));
        }

        if (customer.Addresses.Count >= 3)
        {
            return Result.Failure<AddressSummaryResponse>(Error.Validation("Can't have more than 3 addresses"));
        }

        var address = Domain.Entities.CustomerAddress.Create(
            customerId: customer.Id,
            province: command.Province,
            city: command.City,
            postalAddress: command.PostalAddress,
            postalCode: command.PostalCode,
            latitude: command.Latitude,
            longitude: command.Longitude,
            label: command.Label
        );
        customer.AddAddress(address);
        await addressRepository.AddAsync(address, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new AddressSummaryResponse(
            address.Id,
            Province: address.Province,
            City: address.City,
            PostalAddress: address.PostalAddress,
            Label: address.Label
        ));
    }
}
