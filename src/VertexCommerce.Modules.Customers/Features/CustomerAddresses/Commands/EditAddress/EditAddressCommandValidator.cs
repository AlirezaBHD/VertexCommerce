using FluentValidation;
using VertexCommerce.Modules.Customers.Features.CustomerAddresses.Shared;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.EditAddress;

public sealed class EditAddressCommandValidator : AbstractValidator<EditAddressCommand>
{
    public EditAddressCommandValidator()
    {
        RuleFor(x => x.AddressId).NotEmpty();

        this.ApplyAddressRules();
    }
}
