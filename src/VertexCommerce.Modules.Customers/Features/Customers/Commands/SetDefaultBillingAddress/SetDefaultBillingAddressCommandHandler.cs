using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Commands.SetDefaultBillingAddress;

internal sealed class SetDefaultBillingAddressCommandHandler(
    ICustomerRepository customerRepository,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver,
    ICustomerUnitOfWork unitOfWork)
    : ICommandHandler<SetDefaultBillingAddressCommand>
{
    public async Task<Result> Handle(SetDefaultBillingAddressCommand command, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(userId, ct);
        var customer = await customerRepository.GetByIdAsync(customerId, ct);

        if (customer is null)
        {
            return Result.Failure(Error.NotFound("Customer", userId));
        }

        var address = customer.Addresses.FirstOrDefault(a => a.Id == command.AddressId);

        if (address is null)
        {
            return Result.Failure(Error.NotFound("Address", userId));
        }
        
        customer.SetDefaultBillingAddress(address.Id);
        
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
