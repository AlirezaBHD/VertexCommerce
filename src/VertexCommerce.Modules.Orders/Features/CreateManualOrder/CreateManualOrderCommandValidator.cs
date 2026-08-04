using FluentValidation;

namespace VertexCommerce.Modules.Orders.Features.CreateManualOrder;

public sealed class CreateManualOrderCommandValidator : AbstractValidator<CreateManualOrderCommand>
{
    public CreateManualOrderCommandValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("Customer is required.");

        RuleFor(x => x.ShippingAddress)
            .NotNull().WithMessage("Shipping address is required.")
            .SetValidator(new ManualOrderAddressValidator());

        When(x => x.BillingAddress is not null, () =>
        {
            RuleFor(x => x.BillingAddress!)
                .SetValidator(new ManualOrderAddressValidator());
        });

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one item is required.");

        RuleForEach(x => x.Items)
            .ChildRules(items =>
            {
                items.RuleFor(i => i.ProductId).NotEmpty().WithMessage("Product is required.");
                items.RuleFor(i => i.VariantId).NotEmpty().WithMessage("Variant is required.");
                items.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
            });

        RuleFor(x => x.ShippingCost)
            .GreaterThanOrEqualTo(0).WithMessage("Shipping cost cannot be negative.");

        RuleFor(x => x.Notes)
            .MaximumLength(1000).WithMessage("Notes cannot exceed 1000 characters.");
    }

    private sealed class ManualOrderAddressValidator : AbstractValidator<ManualOrderAddressDto>
    {
        public ManualOrderAddressValidator()
        {
            RuleFor(x => x.Province).NotEmpty().WithMessage("Province is required.");
            RuleFor(x => x.City).NotEmpty().WithMessage("City is required.");
            RuleFor(x => x.PostalAddress).NotEmpty().WithMessage("Postal address is required.");
            RuleFor(x => x.PostalCode).NotEmpty().WithMessage("Postal code is required.");
        }
    }
}
