using FluentValidation;

namespace VertexCommerce.Modules.Orders.Features.ShippingSettings.UpdateShippingSettings;

public sealed class UpdateShippingSettingsCommandValidator : AbstractValidator<UpdateShippingSettingsCommand>
{
    public UpdateShippingSettingsCommandValidator()
    {
        RuleFor(x => x.Cost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Shipping cost cannot be negative.");

        RuleFor(x => x.FreeShippingThreshold)
            .GreaterThan(0)
            .When(x => x.FreeShippingThreshold.HasValue)
            .WithMessage("Free shipping threshold must be greater than zero.");

        RuleFor(x => x.Currency)
            .MaximumLength(3)
            .When(x => !string.IsNullOrWhiteSpace(x.Currency))
            .WithMessage("Currency must be a 3-letter code.");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => x.Description != null);
    }
}
