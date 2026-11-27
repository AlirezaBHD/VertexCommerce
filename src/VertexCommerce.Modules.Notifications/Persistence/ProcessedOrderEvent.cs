namespace VertexCommerce.Modules.Notifications.Persistence;

internal sealed record ProcessedOrderEvent(Guid EventId, DateTime ProcessedAt);
