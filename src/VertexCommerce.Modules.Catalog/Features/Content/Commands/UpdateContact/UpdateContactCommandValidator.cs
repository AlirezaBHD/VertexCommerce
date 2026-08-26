using FluentValidation;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateContact;

public sealed class UpdateContactCommandValidator : AbstractValidator<UpdateContactCommand>
{
    public UpdateContactCommandValidator()
    {
        RuleFor(x => x.Header).NotNull();
        RuleFor(x => x.Location).NotNull();
    }
}
