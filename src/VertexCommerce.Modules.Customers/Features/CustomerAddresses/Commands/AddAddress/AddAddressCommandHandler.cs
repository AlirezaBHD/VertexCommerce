using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Shared;
using VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomer;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AddAddress;

internal sealed class AddAddressCommandHandler(
    ICustomerRepository customerRepository,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver,
    ICustomerUnitOfWork unitOfWork)
    : ICommandHandler<AddAddressCommand, AddressSummaryResponse>
{
    public async Task<Result<AddressSummaryResponse>> Handle(AddAddressCommand command, CancellationToken ct)
    {
        var userId = currentUser.UserId;
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(userId, ct);
        var customer = await customerRepository.GetByIdAsync(customerId, ct);

        if (customer is null)
        {
            return Result.Failure<AddressSummaryResponse>(Error.NotFound("Customer", userId));
        }

        if (!customer.CanAddAddress)
        {
            return Result.Failure<AddressSummaryResponse>(Error.Validation(
                "Customer.TooManyAddresses",
                $"A customer cannot have more than {Customer.MaxAddresses} addresses."));
        }

        var address = CustomerAddress.Create(
            customerId: customer.Id,
            address: command.ToAddress(),
            label: command.ToLabel());

        customer.AddAddress(address);
        customerRepository.AddNewAddress(address);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(AddressSummaryResponse.From(address));
    }
}
