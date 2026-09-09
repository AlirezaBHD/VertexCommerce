using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Domain.Errors;

public static class CustomerErrors
{
    public static Error NotFound(Guid customerId) =>
        Error.NotFound(nameof(Customer), customerId);

    public static Error AddressNotFound(Guid addressId) =>
        Error.NotFound(nameof(CustomerAddress), addressId);

    public static readonly Error TooManyAddresses =
        Error.Validation(
            $"{nameof(Customer)}.{nameof(TooManyAddresses)}",
            $"A customer cannot have more than {Customer.MaxAddresses} addresses.");

    public static readonly Error PhoneExists =
        Error.Validation(
            $"{nameof(Customer)}.{nameof(PhoneExists)}",
            "A customer with this phone number already exists.");
}
