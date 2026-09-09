using FluentValidation;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;

namespace VertexCommerce.Modules.Customers.Features.Customers.Commands.CreateCustomer;

public sealed class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.PhoneNumber).MustInstantiate(PhoneNumber.Create);
        RuleFor(x => x.FirstName).MustInstantiate(FirstName.Create);
        RuleFor(x => x.LastName).MustInstantiate(LastName.Create);
    }
}
