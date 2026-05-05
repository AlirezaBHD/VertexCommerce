using FluentValidation;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.AddAddress;

public sealed class AddAddressCommandValidator : AbstractValidator<AddAddressCommand>
{
    public AddAddressCommandValidator()
    {
        // RuleFor(x => x.Street).NotEmpty().MaximumLength(200);
        // RuleFor(x => x.City).NotEmpty().MaximumLength(100);
        // RuleFor(x => x.State).NotEmpty().MaximumLength(100);
        // RuleFor(x => x.Country).NotEmpty().MaximumLength(100);
        // RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(20);
        // RuleFor(x => x.Label).MaximumLength(50);
    }
}
