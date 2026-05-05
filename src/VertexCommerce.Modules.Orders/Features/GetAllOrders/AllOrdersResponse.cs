namespace VertexCommerce.Modules.Orders.Features.GetAllOrders;
    
public sealed record AllOrdersResponse(
    Guid Id,
    string CustomerPhoneNumber,
    string OrderNumber,
    string Status,
    string PaymentStatus,
    string TotalAmount,
    string? TrackingNumber,
    DateTime? CreatedAt,
    DateTime? UpdatedAt
);