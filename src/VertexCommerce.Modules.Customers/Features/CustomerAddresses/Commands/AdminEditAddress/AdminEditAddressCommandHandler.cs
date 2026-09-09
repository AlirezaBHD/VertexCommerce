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

        var address = customer.FindAddress(command.AddressId);

        if (address is null)
        {
            return Result.Failure(Error.NotFound("Address", command.AddressId));
        }

        address.Relocate(command.ToAddress(), command.ToLabel());

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
