using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Orders.Domain.Events;

public sealed record OrderCreatedEvent(
    Guid OrderId,
    Guid CustomerId,
    string OrderNumber,
    decimal TotalAmount,
    string Currency
) : DomainEvent;
