using VertexCommerce.Modules.Notifications.Domain.Entities;

namespace VertexCommerce.Modules.Notifications.Services.Dispatching;

internal sealed record InboxNotification(Notification Notification, string PushPayload);
