using System.Buffers;
using System.Buffers.Text;
using FluentValidation;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Services;

namespace VertexCommerce.Modules.Notifications.Features.PushSubscriptions.Commands.SubscribeToPush;

public sealed class SubscribeToPushCommandValidator : AbstractValidator<SubscribeToPushCommand>
{
    public SubscribeToPushCommandValidator()
    {
        RuleFor(command => command.Endpoint)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(PushSubscriptionLimits.Endpoint)
            .Must(WebPushEndpointPolicy.IsAllowed).WithMessage("Endpoint must use HTTPS on a supported Web Push provider.");

        RuleFor(command => command.P256dh)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(PushSubscriptionLimits.P256dhKey)
            .Must(IsPublicKey).WithMessage("P256dh must be an uncompressed P-256 public key in base64url format.");

        RuleFor(command => command.Auth)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .MaximumLength(PushSubscriptionLimits.AuthKey)
            .Must(IsAuthSecret).WithMessage("Auth must be a 16-byte secret in base64url format.");

        RuleFor(command => command.UserAgent).MaximumLength(PushSubscriptionLimits.UserAgent);
    }

    private static bool IsPublicKey(string value)
    {
        Span<byte> key = stackalloc byte[65];
        return Base64Url.DecodeFromChars(value.AsSpan().Trim(), key, out _, out var length) == OperationStatus.Done &&
            length == key.Length && key[0] == 4;
    }

    private static bool IsAuthSecret(string value)
    {
        Span<byte> secret = stackalloc byte[16];
        return Base64Url.DecodeFromChars(value.AsSpan().Trim(), secret, out _, out var length) == OperationStatus.Done &&
            length == secret.Length;
    }
}
