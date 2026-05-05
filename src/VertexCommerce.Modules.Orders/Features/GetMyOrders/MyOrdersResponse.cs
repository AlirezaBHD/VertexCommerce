namespace VertexCommerce.Modules.Orders.Features.GetMyOrders;

public sealed record MyOrdersResponse(
    Guid Id,
    string OrderNumber, 
    string Status,
    string PaymentStatus,
    string SubTotal,
    string TotalAmount,
    string? TrackingNumber,
    string ShippingAddress);