using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminSetDefaultAddress;

internal sealed class AdminSetDefaultAddressCommandHandler(
    ICustomerRepository customerRepository,
    ICustomerUnitOfWork unitOfWork)
    : ICommandHandler<AdminSetDefaultAddressCommand>
{
    public async Task<Result> Handle(AdminSetDefaultAddressCommand command, CancellationToken ct)
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

        customer.SetDefaultShippingAddress(address.Id);
        customer.SetDefaultBillingAddress(address.Id);

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
