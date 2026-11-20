using FluentValidation;

namespace VertexCommerce.Modules.Notifications.Features.PushSubscriptions.Commands.SubscribeToPush;

public sealed class SubscribeToPushCommandValidator : AbstractValidator<SubscribeToPushCommand>
{
    public SubscribeToPushCommandValidator()
    {
        RuleFor(x => x.Endpoint)
            .NotEmpty().WithMessage("Endpoint is required.");

        RuleFor(x => x.P256dh)
            .NotEmpty().WithMessage("P256dh key is required.");

        RuleFor(x => x.Auth)
            .NotEmpty().WithMessage("Auth key is required.");
    }
}
