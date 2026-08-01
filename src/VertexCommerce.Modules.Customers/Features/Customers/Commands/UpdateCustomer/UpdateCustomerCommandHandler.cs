using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Modules.Customers.Persistence;
using VertexCommerce.Shared.CQRS;

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
                Error.NotFound("Customer", command.CustomerId));
        }

        var phoneNumber = command.PhoneNumber.Trim();

        if (!string.Equals(customer.PhoneNumber, phoneNumber, StringComparison.Ordinal))
        {
            var existing = await customerRepository.GetByPhoneNumberAsync(phoneNumber, ct);
            if (existing is not null)
            {
                return Result.Failure<UpdateCustomerResponse>(
                    Error.Validation("Customer.PhoneExists", "A customer with this phone number already exists."));
            }
        }

        customer.UpdateProfile(
            phoneNumber: phoneNumber,
            firstName: command.FirstName.Trim(),
            lastName: command.LastName.Trim());

        customerRepository.Update(customer);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new UpdateCustomerResponse(
            customer.Id,
            customer.PhoneNumber,
            customer.FirstName,
            customer.LastName));
    }
}
