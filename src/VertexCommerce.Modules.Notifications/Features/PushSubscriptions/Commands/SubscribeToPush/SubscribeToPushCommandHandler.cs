using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Persistence;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.PushSubscriptions.Commands.SubscribeToPush;

internal sealed class SubscribeToPushCommandHandler(
    IPushSubscriptionRepository repository,
    INotificationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : ICommandHandler<SubscribeToPushCommand>
{
    public async Task<Result> Handle(SubscribeToPushCommand command, CancellationToken ct)
    {
        var subscription = PushSubscription.Create(
            userId: currentUser.GetUserId(),
            endpoint: command.Endpoint,
            p256DhKey: command.P256dh,
            authKey: command.Auth,
            userAgent: command.UserAgent);

        await repository.AddOrUpdateAsync(subscription, ct);
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
