using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomer;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.RemoveAddress;

internal sealed class RemoveAddressCommandHandler(
    ICustomerAddressRepository addressRepository,
    ICustomerUnitOfWork unitOfWork,
    ICurrentUser currentUser,
    ICustomerResolver  customerResolver)
    : ICommandHandler<RemoveAddressCommand>
{
    public async Task<Result> Handle(RemoveAddressCommand command, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(userId, ct);

        if (customerId == Guid.Empty)
        {
            return Result.Failure<AddressSummaryResponse>(Error.NotFound("Customer", userId));
        }

        var address =
            await addressRepository.GetAsync(addressId: command.AddressId, customerId: customerId, ct);

        if (address is null)
        {
            return Result.Failure<AddressSummaryResponse>(Error.NotFound("Address", command.AddressId));
        }

        address.SoftDelete();

        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
