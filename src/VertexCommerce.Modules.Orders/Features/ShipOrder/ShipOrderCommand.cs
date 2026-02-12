using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.ShipOrder;

public sealed record ShipOrderCommand(Guid OrderId, string TrackingNumber) : ICommand;
