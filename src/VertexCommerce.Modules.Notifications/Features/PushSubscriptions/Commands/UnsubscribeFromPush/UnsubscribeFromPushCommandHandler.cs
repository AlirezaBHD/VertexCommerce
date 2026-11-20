using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.PushSubscriptions.Commands.UnsubscribeFromPush;

internal sealed class UnsubscribeFromPushCommandHandler(
    IPushSubscriptionRepository repository)
    : ICommandHandler<UnsubscribeFromPushCommand>
{
    public async Task<Result> Handle(UnsubscribeFromPushCommand command, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(command.Endpoint))
        {
            await repository.RemoveByEndpointAsync(command.Endpoint, ct);
        }

        return Result.Success();
    }
}
