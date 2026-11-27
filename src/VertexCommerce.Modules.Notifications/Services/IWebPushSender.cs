using VertexCommerce.Modules.Notifications.Domain.Entities;
using VertexCommerce.Modules.Notifications.Services.Delivery;

namespace VertexCommerce.Modules.Notifications.Services;

internal interface IWebPushSender
{
    Task<WebPushSendResult> SendAsync(
        Guid deliveryId, string payload, DateTimeOffset expiresAt, PushSubscription subscription, CancellationToken ct);
}
