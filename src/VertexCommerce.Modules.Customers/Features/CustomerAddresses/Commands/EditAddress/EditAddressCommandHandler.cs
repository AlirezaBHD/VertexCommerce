using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Shared;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.EditAddress;

internal sealed class EditAddressCommandHandler(
    ICustomerRepository customerRepository,
    ICustomerUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver)
    : ICommandHandler<EditAddressCommand>
{
    public async Task<Result> Handle(EditAddressCommand command, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(userId, ct);
        var customer = await customerRepository.GetByIdAsync(customerId, ct);

        if (customer is null)
        {
            return Result.Failure(Error.NotFound("Customer", userId));
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
