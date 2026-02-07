using VertexCommerce.Modules.Orders.Domain.Enums;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Orders.Domain.Events;

public sealed record OrderStatusChangedEvent(
    Guid OrderId,
    OrderStatus OldStatus,
    OrderStatus NewStatus
) : DomainEvent;
