using VertexCommerce.Modules.Notifications.Services.Delivery;

namespace VertexCommerce.Modules.Notifications.Persistence;

internal sealed class PushDelivery(
    Guid id, Guid notificationId, Guid subscriptionId, string payload, DateTimeOffset nextAttemptAt, DateTimeOffset expiresAt)
{
    public Guid Id { get; } = id;
    public Guid NotificationId { get; } = notificationId;
    public Guid SubscriptionId { get; } = subscriptionId;
    public string Payload { get; } = payload;
    public DateTimeOffset NextAttemptAt { get; init; } = nextAttemptAt;
    public DateTimeOffset ExpiresAt { get; } = expiresAt;
    public int Attempts { get; init; }
    public Guid? LeaseId { get; init; }
    public DateTimeOffset? LeaseExpiresAt { get; init; }
    public DateTimeOffset? CompletedAt { get; init; }
    public PushDeliveryOutcome? Outcome { get; init; }
    public int? LastHttpStatus { get; init; }
}
