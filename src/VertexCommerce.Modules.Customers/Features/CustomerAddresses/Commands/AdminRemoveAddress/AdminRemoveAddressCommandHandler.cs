using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminRemoveAddress;

internal sealed class AdminRemoveAddressCommandHandler(
    ICustomerAddressRepository addressRepository,
    ICustomerRepository customerRepository,
    ICustomerUnitOfWork unitOfWork)
    : ICommandHandler<AdminRemoveAddressCommand>
{
    public async Task<Result> Handle(AdminRemoveAddressCommand command, CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdAsync(command.CustomerId, ct);

        if (customer is null)
        {
            return Result.Failure(Error.NotFound("Customer", command.CustomerId));
        }

        var address = await addressRepository.GetAsync(addressId: command.AddressId, customerId: command.CustomerId, ct);

        if (address is null)
        {
            return Result.Failure(Error.NotFound("Address", command.AddressId));
        }

        customer.RemoveAddress(command.AddressId);
        address.SoftDelete();

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
