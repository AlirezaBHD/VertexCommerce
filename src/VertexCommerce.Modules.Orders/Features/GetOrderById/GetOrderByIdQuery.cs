using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.GetOrderById;

public sealed record GetOrderByIdQuery(Guid Id) : IQuery<OrderResponse>;
