using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.PushSubscriptions.Commands.UnsubscribeFromPush;

internal sealed class UnsubscribeFromPushCommandHandler(
    IPushSubscriptionRepository repository,
    ICurrentUser currentUser)
    : ICommandHandler<UnsubscribeFromPushCommand>
{
    public async Task<Result> Handle(UnsubscribeFromPushCommand command, CancellationToken ct)
    {
        if (command.Endpoint?.Trim() is { Length: > 0 and <= PushSubscriptionLimits.Endpoint } endpoint)
        {
            await repository.RemoveOwnedByEndpointAsync(currentUser.GetUserId(), endpoint, ct);
        }

        return Result.Success();
    }
}
