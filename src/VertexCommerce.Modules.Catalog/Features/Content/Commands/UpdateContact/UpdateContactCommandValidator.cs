using FluentValidation;
using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateContact;

public sealed class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactCommandValidator()
    {
        RuleFor(x => x.Header).NotNull();
        RuleFor(x => x.Location).NotNull();

        When(x => x.Location != null && !string.IsNullOrWhiteSpace(x.Location.PostalCode), () =>
        {
            RuleFor(x => x.Location.PostalCode)
                .Must(pc =>
                {
                    var normalized = DigitNormalization.ToAsciiDigits(pc.Trim());
                    return normalized.Length == 10 && normalized.All(char.IsAsciiDigit);
                })
                .WithMessage("Postal code must be exactly 10 digits.");
        });
    }
}
