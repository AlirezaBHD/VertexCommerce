using FluentValidation;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AddAddress;

public sealed class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator()
    {
        this.ApplyAddressRules();
    }
}
