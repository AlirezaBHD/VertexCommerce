using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.ConfirmOrder;

public sealed record ConfirmOrderCommand(Guid OrderId) : ICommand;
