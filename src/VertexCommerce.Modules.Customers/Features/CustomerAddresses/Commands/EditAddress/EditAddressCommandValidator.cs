using FluentValidation;
using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Shared.Domain;
using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Commands.EditAddress;

public sealed class EditAddressCommandValidator : AbstractValidator<EditAddressCommand>
{
    public EditAddressCommandValidator()
    {
        RuleFor(x => x.AddressId)
            .NotEmpty();

        RuleFor(x => x.Province)
            .HasSpec(StringField.Of<CustomerAddress>(a => a.Province));

        RuleFor(x => x.City)
            .HasSpec(StringField.Of<CustomerAddress>(a => a.City));

        RuleFor(x => x.PostalAddress)
            .HasSpec(StringField.Of<CustomerAddress>(a => a.PostalAddress));

        RuleFor(x => x.PostalCode)
            .HasSpec(StringField.Of<CustomerAddress>(a => a.PostalCode));

        RuleFor(x => x.Label)
            .HasSpec(StringField.Of<CustomerAddress>(a => a.Label));
    }
}


