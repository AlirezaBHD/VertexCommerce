namespace VertexCommerce.Modules.Notifications.Services.Delivery;

internal enum WebPushSendStatus
{
    Accepted = 0,
    Retryable = 1,
    SubscriptionExpired = 2,
    InvalidSubscription = 3,
    Rejected = 4,
}
