using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.ProcessOrder;

public sealed record ProcessOrderCommand(Guid OrderId) : ICommand;
