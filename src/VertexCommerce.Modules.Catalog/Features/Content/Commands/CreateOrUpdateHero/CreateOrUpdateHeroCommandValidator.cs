using FluentValidation;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateHero;

public sealed class CreateOrUpdateHeroCommandValidator : AbstractValidator<CreateOrUpdateHeroCommand>
{
    public CreateOrUpdateHeroCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.RedirectPath)
            .NotEmpty().WithMessage("RedirectPath is required.");
    }
}
