using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Modules.Customers.Domain.Errors;

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
        var phoneNumber = PhoneNumber.Create(command.PhoneNumber);

        var existing = await customerRepository.GetByPhoneNumberAsync(phoneNumber, ct);
        if (existing is not null)
        {
            return Result.Failure<CreateCustomerResponse>(
                CustomerErrors.PhoneExists);
        }

        var customer = Customer.Create(
            userId: null,
            phoneNumber: phoneNumber,
            firstName: FirstName.Create(command.FirstName),
            lastName: LastName.Create(command.LastName));

        await customerRepository.AddAsync(customer, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new CreateCustomerResponse(
            customer.Id,
            customer.PhoneNumber.Value,
            customer.FirstName.Value,
            customer.LastName.Value));
    }
}
