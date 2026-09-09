using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Queries.GetAddressById;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Shared;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminAddAddress;

internal sealed class AdminAddAddressCommandHandler(
    ICustomerRepository customerRepository,
    ICustomerUnitOfWork unitOfWork)
    : ICommandHandler<AdminAddAddressCommand, AddressResponse>
{
    public async Task<Result<AddressResponse>> Handle(AdminAddAddressCommand command, CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdAsync(command.CustomerId, ct);

        if (customer is null)
        {
            return Result.Failure<AddressResponse>(Error.NotFound("Customer", command.CustomerId));
        }

        if (!customer.CanAddAddress)
        {
            return Result.Failure<AddressResponse>(Error.Validation(
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

        return Result.Success(AddressResponse.From(address));
    }
}
