using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Queries.GetAddressById;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminAddAddress;

internal sealed class AdminAddAddressCommandHandler(
    ICustomerRepository customerRepository,
    ICustomerAddressRepository addressRepository,
    ICustomerUnitOfWork unitOfWork)
    : ICommandHandler<AdminAddAddressCommand, AddressResponse>
{
    public async Task<Result<AddressResponse>> Handle(AdminAddAddressCommand command, CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdAsync(command.CustomerId, ct);

        if (customer is null)
        {
            return Result.Failure<AddressResponse>(Error.NotFound("Customer", command.CustomerId));
        }

        var address = CustomerAddress.Create(
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

        return Result.Success(new AddressResponse(
            Id: address.Id,
            CustomerId: address.CustomerId,
            Province: address.Province,
            City: address.City,
            PostalAddress: address.PostalAddress,
            PostalCode: address.PostalCode,
            Latitude: address.Latitude,
            Longitude: address.Longitude,
            Label: address.Label,
            CreatedAt: address.CreatedAt
        ));
    }
}
