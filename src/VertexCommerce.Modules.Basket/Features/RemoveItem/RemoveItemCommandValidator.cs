using FluentValidation;

namespace VertexCommerce.Modules.Basket.Features.RemoveItem;

public sealed class RemoveItemCommandValidator : AbstractValidator<RemoveItemCommand>
{
    public RemoveItemCommandValidator()
    {
        RuleFor(x => x.VariantId)
            .NotEmpty().WithMessage("Variant ID is required.");
    }
}
