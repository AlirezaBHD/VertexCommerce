using VertexCommerce.Modules.Notifications.Features.Notifications.DTOs;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.Notifications.Queries.GetNotifications;

public sealed record GetNotificationsQuery(int Page = 1, int PageSize = 20) : IQuery<IReadOnlyList<NotificationDto>>;
