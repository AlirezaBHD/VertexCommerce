using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Modules.Customers.Domain.Errors;

namespace VertexCommerce.Modules.Customers.Features.Customers.Commands.UpdateCustomer;

internal sealed class UpdateCustomerCommandHandler(
    ICustomerRepository customerRepository,
    ICustomerUnitOfWork unitOfWork)
    : ICommandHandler<UpdateCustomerCommand, UpdateCustomerResponse>
{
    public async Task<Result<UpdateCustomerResponse>> Handle(
        UpdateCustomerCommand command,
        CancellationToken ct)
    {
        var customer = await customerRepository.GetByIdAsync(command.CustomerId, ct);

        if (customer is null)
        {
            return Result.Failure<UpdateCustomerResponse>(
                CustomerErrors.NotFound(command.CustomerId));
        }

        // Normalization now happens inside the value object, so comparing the two is enough.
        var phoneNumber = PhoneNumber.Create(command.PhoneNumber);

        if (customer.PhoneNumber != phoneNumber)
        {
            var existing = await customerRepository.GetByPhoneNumberAsync(phoneNumber, ct);
            if (existing is not null)
            {
                return Result.Failure<UpdateCustomerResponse>(
                    CustomerErrors.PhoneExists);
            }
        }

        customer.UpdateProfile(
            phoneNumber: phoneNumber,
            firstName: FirstName.Create(command.FirstName),
            lastName: LastName.Create(command.LastName));

        customerRepository.Update(customer);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new UpdateCustomerResponse(
            customer.Id,
            customer.PhoneNumber.Value,
            customer.FirstName.Value,
            customer.LastName.Value));
    }
}
