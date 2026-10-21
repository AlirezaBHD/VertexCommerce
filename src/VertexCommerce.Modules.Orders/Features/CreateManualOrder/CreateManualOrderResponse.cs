namespace VertexCommerce.Modules.Orders.Features.CreateManualOrder;

internal sealed record CreateManualOrderResponse(
    Guid OrderId,
    string OrderNumber,
    decimal TotalAmount,
    string Currency);