using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Queries.GetAddressById;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Shared;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Modules.Customers.Domain.Errors;

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
            return Result.Failure<AddressResponse>(CustomerErrors.NotFound(command.CustomerId));
        }

        if (!customer.CanAddAddress)
        {
            return Result.Failure<AddressResponse>(CustomerErrors.TooManyAddresses);
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
