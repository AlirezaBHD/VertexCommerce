using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.InitiatePayment;

public sealed record InitiatePaymentCommand(Guid OrderId) : ICommand;
