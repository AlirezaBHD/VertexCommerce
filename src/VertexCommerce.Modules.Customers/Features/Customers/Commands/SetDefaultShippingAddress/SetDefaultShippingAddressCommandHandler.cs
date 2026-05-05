using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AddAddress;
using VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomer;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Commands.SetDefaultShippingAddress;

internal sealed class SetDefaultShippingAddressCommandHandler(
    ICustomerRepository customerRepository,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver,
    ICustomerUnitOfWork unitOfWork)
    : ICommandHandler<SetDefaultShippingAddressCommand>
{
    public async Task<Result> Handle(SetDefaultShippingAddressCommand command, CancellationToken ct)
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
        
        customer.SetDefaultShippingAddress(address.Id);
        
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
