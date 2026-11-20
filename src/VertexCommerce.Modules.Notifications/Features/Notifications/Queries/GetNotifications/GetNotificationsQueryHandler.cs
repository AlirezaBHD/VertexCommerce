using VertexCommerce.Modules.Notifications.Domain.Repositories;
using VertexCommerce.Modules.Notifications.Features.Notifications.DTOs;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.Notifications.Queries.GetNotifications;

internal sealed class GetNotificationsQueryHandler(
    INotificationRepository repository,
    ICurrentUser currentUser)
    : IQueryHandler<GetNotificationsQuery, IReadOnlyList<NotificationDto>>
{
    public async Task<Result<IReadOnlyList<NotificationDto>>> Handle(GetNotificationsQuery query, CancellationToken ct)
    {
        var userId = currentUser.GetUserId();
        var notifications = await repository.GetUserNotificationsAsync(userId, query.Page, query.PageSize, ct);

        var dtos = notifications.Select(n => new NotificationDto(
            n.Id,
            n.UserId,
            n.Title,
            n.Message,
            n.Type,
            n.PayloadJson,
            n.IsRead,
            n.ReadAt,
            n.CreatedAt)).ToList();

        return Result.Success<IReadOnlyList<NotificationDto>>(dtos);
    }
}
