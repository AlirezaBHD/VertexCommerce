namespace VertexCommerce.Modules.Orders.Features.Dashboard.GetRecentOrders;

public sealed record RecentOrderResponse(
    Guid Id,
    string OrderNumber,
    string CustomerPhoneNumber,
    string Status,
    string TotalAmount,
    DateTime CreatedAt);
