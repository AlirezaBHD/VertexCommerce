using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Notifications.Features.Notifications.Queries.GetUnreadCount;

public sealed record GetUnreadCountQuery : IQuery<int>;
