using FluentValidation;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.ReorderCategories;

public sealed class ReorderCategoriesCommandValidator : AbstractValidator<ReorderCategoriesCommand>
{
    public ReorderCategoriesCommandValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("At least one category reorder item is required.");

        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Category ID is required.");

            item.RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("Sort order cannot be negative.");
        });
    }
}
