using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.DeliverOrder;

public sealed record DeliverOrderCommand(Guid OrderId) : ICommand;
