using FluentValidation;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminEditAddress;

public sealed class AdminEditAddressCommandValidator : AbstractValidator<AdminEditAddressCommand>
{
    public AdminEditAddressCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();

        RuleFor(x => x.AddressId)
            .NotEmpty();

        RuleFor(x => x.Province)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.PostalAddress)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .MaximumLength(20);
    }
}
