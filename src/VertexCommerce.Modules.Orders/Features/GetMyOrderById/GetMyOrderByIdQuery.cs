using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.GetMyOrderById;

public sealed record GetMyOrderByIdQuery(Guid OrderId) : IQuery<MyOrderResponse>;
