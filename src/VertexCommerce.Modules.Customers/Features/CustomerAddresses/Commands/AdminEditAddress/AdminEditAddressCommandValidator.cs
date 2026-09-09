using FluentValidation;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Shared;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminEditAddress;

public sealed class AdminEditAddressCommandValidator : AbstractValidator<AdminEditAddressCommand>
{
    public AdminEditAddressCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();
        RuleFor(x => x.AddressId).NotEmpty();

        this.ApplyAddressRules();
    }
}
