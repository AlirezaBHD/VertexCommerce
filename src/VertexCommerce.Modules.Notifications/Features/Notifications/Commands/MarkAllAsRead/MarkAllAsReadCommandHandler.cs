using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.Notifications.Commands.MarkAllAsRead;

internal sealed class MarkAllAsReadCommandHandler(
    INotificationRepository repository,
    ICurrentUser currentUser)
    : ICommandHandler<MarkAllAsReadCommand>
{
    public async Task<Result> Handle(MarkAllAsReadCommand command, CancellationToken ct)
    {
        await repository.MarkAllAsReadAsync(currentUser.GetUserId(), ct);
        return Result.Success();
    }
}
