using FluentValidation;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;

public sealed class UpdateAboutCommandValidator : AbstractValidator<UpdateAboutCommand>
{
    public UpdateAboutCommandValidator()
    {
        // Validation rules are flexible to allow custom partial updates
    }
}
