using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.CancelOrder;

public sealed record CancelOrderCommand(Guid OrderId, string Reason) : ICommand;
