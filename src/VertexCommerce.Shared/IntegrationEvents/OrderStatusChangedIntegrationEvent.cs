using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Shared.IntegrationEvents;

public sealed record OrderStatusChangedIntegrationEvent(
    Guid OrderId,
    Guid CustomerId,
    string OrderNumber,
    string OldStatus,
    string NewStatus,
    decimal TotalAmount,
    string Currency,
    string? TrackingNumber = null,
    string? CancellationReason = null
) : DomainEvent;
