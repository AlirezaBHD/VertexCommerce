using FluentValidation;
using VertexCommerce.Modules.Identity.Domain.ValueObjects;

namespace VertexCommerce.Modules.Identity.Features.Commands.Registration.CompleteRegistration;

public sealed class CompleteRegistrationCommandValidator : AbstractValidator<CompleteRegistrationCommand>
{
    public CompleteRegistrationCommandValidator()
    {
        RuleFor(x => x.RegistrationToken)
            .NotEmpty().WithMessage("Registration token is required.");

        RuleFor(x => x.FirstName)
            .MustInstantiate(FirstName.Create);

        RuleFor(x => x.LastName)
            .MustInstantiate(LastName.Create);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters.");
    }
}
