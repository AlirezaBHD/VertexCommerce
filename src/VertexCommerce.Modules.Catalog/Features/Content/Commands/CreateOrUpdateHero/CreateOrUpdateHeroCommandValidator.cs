using FluentValidation;
using VertexCommerce.Modules.Catalog.Domain.Banners;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateHero;

public sealed class CreateOrUpdateHeroCommandValidator : AbstractValidator<CreateOrUpdateHeroCommand>
{
    public CreateOrUpdateHeroCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Target)
            .NotNull().WithMessage("Target is required.");

        RuleFor(x => x.Target.Type)
            .IsInEnum().WithMessage("Invalid target type.");

        When(x => x.Target.Type == TargetType.Product, () =>
        {
            RuleFor(x => x.Target.ProductId)
                .NotNull().WithMessage("ProductId is required for Product target.");
        });

        When(x => x.Target.Type == TargetType.Category, () =>
        {
            RuleFor(x => x.Target.CategoryId)
                .NotNull().WithMessage("CategoryId is required for Category target.");
        });

        When(x => x.Target.Type == TargetType.InternalPath, () =>
        {
            RuleFor(x => x.Target.InternalPath)
                .NotEmpty().WithMessage("InternalPath is required for InternalPath target.");
        });

        When(x => x.Target.Type == TargetType.ExternalUrl, () =>
        {
            RuleFor(x => x.Target.ExternalUrl)
                .NotEmpty().WithMessage("ExternalUrl is required for ExternalUrl target.");
        });
    }
}
