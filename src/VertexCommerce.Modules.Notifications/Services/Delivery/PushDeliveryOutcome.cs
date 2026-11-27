namespace VertexCommerce.Modules.Notifications.Services.Delivery;

internal enum PushDeliveryOutcome
{
    Accepted = 0,
    SubscriptionRemoved = 1,
    SubscriptionExpired = 2,
    InvalidSubscription = 3,
    Rejected = 4,
    AttemptsExhausted = 5,
    Expired = 6,
}
