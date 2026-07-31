using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Commands.CreateCustomer;

internal sealed class CreateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    ICustomerUnitOfWork unitOfWork)
    : ICommandHandler<CreateCustomerCommand, CreateCustomerResponse>
{
    public async Task<Result<CreateCustomerResponse>> Handle(
        CreateCustomerCommand command,
        CancellationToken ct)
    {
        var existing = await customerRepository.GetByPhoneNumberAsync(command.PhoneNumber, ct);
        if (existing is not null)
        {
            return Result.Failure<CreateCustomerResponse>(
                Error.Validation("Customer.PhoneExists", "A customer with this phone number already exists."));
        }

        var customer = Customer.Create(
            userId: null,
            phoneNumber: command.PhoneNumber,
            firstName: command.FirstName,
            lastName: command.LastName);

        await customerRepository.AddAsync(customer, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new CreateCustomerResponse(
            customer.Id,
            customer.PhoneNumber,
            customer.FirstName,
            customer.LastName));
    }
}
