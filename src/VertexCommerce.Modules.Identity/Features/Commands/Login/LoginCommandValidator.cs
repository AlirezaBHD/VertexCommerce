using FluentValidation;
using VertexCommerce.Modules.Identity.Domain.ValueObjects;

namespace VertexCommerce.Modules.Identity.Features.Commands.Login;

public sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .MustInstantiate(PhoneNumber.Create);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}
