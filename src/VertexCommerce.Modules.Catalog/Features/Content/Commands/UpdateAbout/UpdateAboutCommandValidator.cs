using FluentValidation;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;

public sealed class UpdateAboutCommandValidator : AbstractValidator<UpdateAboutCommand>
{
    public UpdateAboutCommandValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(5000);
    }
}
