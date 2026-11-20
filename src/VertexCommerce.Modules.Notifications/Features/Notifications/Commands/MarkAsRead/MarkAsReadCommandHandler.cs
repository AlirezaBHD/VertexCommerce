using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Persistence;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.Notifications.Commands.MarkAsRead;

internal sealed class MarkAsReadCommandHandler(
    INotificationRepository repository,
    INotificationsUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : ICommandHandler<MarkAsReadCommand>
{
    public async Task<Result> Handle(MarkAsReadCommand command, CancellationToken ct)
    {
        var notification = await repository.GetByIdAsync(command.NotificationId, ct);
        if (notification is null || notification.UserId != currentUser.GetUserId())
        {
            return Result.Failure(Error.NotFound("Notification", command.NotificationId));
        }

        notification.MarkAsRead();
        await unitOfWork.SaveChangesAsync(ct);

        return Result.Success();
    }
}
