using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Orders.Domain.Events;

public sealed record OrderCancelledEvent(
    Guid OrderId,
    string OrderNumber,
    string Reason
) : DomainEvent;
