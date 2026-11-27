using Microsoft.Extensions.Options;
using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Services;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.PushSubscriptions.Commands.SubscribeToPush;

internal sealed class SubscribeToPushCommandHandler(
    IPushSubscriptionRepository repository,
    ICurrentUser currentUser,
    IOptions<WebPushOptions> options)
    : ICommandHandler<SubscribeToPushCommand>
{
    public async Task<Result> Handle(SubscribeToPushCommand command, CancellationToken ct)
    {
        if (!options.Value.IsEnabled)
        {
            return Result.Failure(WebPushErrors.Unavailable);
        }

        var subscription = PushSubscription.Create(
            userId: currentUser.GetUserId(),
            endpoint: command.Endpoint,
            p256DhKey: command.P256dh,
            authKey: command.Auth,
            userAgent: command.UserAgent);

        var saved = await repository.TryUpsertOwnedAsync(subscription, ct);
        return saved
            ? Result.Success()
            : Result.Failure(Error.Conflict("Push subscription belongs to another account."));
    }
}
