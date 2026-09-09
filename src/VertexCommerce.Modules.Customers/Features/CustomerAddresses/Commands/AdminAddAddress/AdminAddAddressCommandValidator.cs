using FluentValidation;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AdminAddAddress;

public sealed class AdminAddAddressCommandValidator : AbstractValidator<AdminAddAddressCommand>
{
    public AdminAddAddressCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty();

        this.ApplyAddressRules();
    }
}
