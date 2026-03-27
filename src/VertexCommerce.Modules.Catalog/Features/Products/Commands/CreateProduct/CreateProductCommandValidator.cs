using FluentValidation;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.")
            .When(x => x.Description is not null);

        // RuleFor(x => x.Sku)
        //     .MinimumLength(3).WithMessage("SKU must be at least 3 characters.")
        //     .MaximumLength(50).WithMessage("SKU cannot exceed 50 characters.")
        //     .Matches(@"^[A-Za-z0-9\-]+$").WithMessage("SKU can only contain letters, numbers, and hyphens.")
        //     .When(x => x.Sku is not null);
        //
        // RuleFor(x => x.Price)
        //     .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");
        //
        // RuleFor(x => x.Currency)
        //     .NotEmpty().WithMessage("Currency is required.")
        //     .Length(3).WithMessage("Currency must be 3 characters (e.g., USD, EUR).");
        //
        // RuleFor(x => x.StockQuantity)
        //     .GreaterThanOrEqualTo(0).WithMessage("Stock quantity cannot be negative.");

        RuleFor(x => x.CategoryId)
            .NotEmpty().WithMessage("Category is required.");
    }
}
