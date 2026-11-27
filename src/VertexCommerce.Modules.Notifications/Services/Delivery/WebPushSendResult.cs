namespace VertexCommerce.Modules.Notifications.Services.Delivery;

internal sealed record WebPushSendResult(
    WebPushSendStatus Status, int? HttpStatus = null, DateTimeOffset? RetryAfter = null);
