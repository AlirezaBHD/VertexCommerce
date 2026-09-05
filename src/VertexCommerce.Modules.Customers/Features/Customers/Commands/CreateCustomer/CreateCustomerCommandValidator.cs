using FluentValidation;
using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Shared.Domain;
using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Customers.Features.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .HasSpec(StringField.Of<Customer>(c => c.PhoneNumber));

        RuleFor(x => x.FirstName)
            .HasSpec(StringField.Of<Customer>(c => c.FirstName));

        RuleFor(x => x.LastName)
            .HasSpec(StringField.Of<Customer>(c => c.LastName));
    }
}


