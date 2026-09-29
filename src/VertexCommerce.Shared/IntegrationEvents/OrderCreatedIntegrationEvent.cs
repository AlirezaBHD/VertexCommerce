using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Shared.IntegrationEvents;

public sealed record OrderCreatedIntegrationEvent(
    Guid OrderId,
    Guid CustomerId,
    string OrderNumber,
    decimal TotalAmount,
    string Currency
) : DomainEvent;
