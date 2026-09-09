using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.RemoveAddress;

internal sealed class RemoveAddressCommandHandler(
    ICustomerRepository customerRepository,
    ICustomerUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : ICommandHandler<RemoveAddressCommand>
{
    public async Task<Result> Handle(RemoveAddressCommand command, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var customer = await customerRepository.GetByUserIdAsync(userId, ct);

        if (customer is null)
        {
            return Result.Failure(Error.NotFound("Customer", userId));
        }

        var address = customer.FindAddress(command.AddressId);
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
