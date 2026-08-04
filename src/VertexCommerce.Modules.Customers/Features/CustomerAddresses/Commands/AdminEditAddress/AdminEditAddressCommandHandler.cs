using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminEditAddress;

internal sealed class AdminEditAddressCommandHandler(
    ICustomerRepository customerRepository,
    ICustomerUnitOfWork unitOfWork)
    : ICommandHandler<AdminEditAddressCommand>
{
    public async Task<Result> Handle(AdminEditAddressCommand command, CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdAsync(command.CustomerId, ct);

        if (customer is null)
        {
            return Result.Failure(Error.NotFound("Customer", command.CustomerId));
        }

        var address = customer.Addresses.FirstOrDefault(a => a.Id == command.AddressId);

        if (address is null)
        {
            return Result.Failure(Error.NotFound("Address", command.AddressId));
        }

        address.Update(
            province: command.Province,
            city: command.City,
            postalAddress: command.PostalAddress,
            postalCode: command.PostalCode,
            latitude: command.Latitude,
            longitude: command.Longitude,
            label: command.Label);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
