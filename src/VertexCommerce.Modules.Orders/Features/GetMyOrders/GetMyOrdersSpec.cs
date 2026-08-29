using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Orders.Features.GetMyOrders;

public sealed class GetMyOrdersSpec : BaseSpecification<Order, MyOrdersResponse>
{
    public GetMyOrdersSpec(Guid currentUserId)
    {
        Where(o => o.CustomerId == currentUserId);

        OrderByDesc(o => o.CreatedAt);

        Select(o => new MyOrdersResponse(
            Id: o.Id,
            OrderNumber: o.OrderNumber,
            Status: o.Status.ToString(),
            PaymentStatus: o.PaymentStatus.ToString(),
            SubTotal: o.SubTotal.ToString(),
            TotalAmount: o.TotalAmount.ToString(),
            TrackingNumber: o.TrackingNumber,
            ShippingAddress: o.ShippingAddress.ToStringSummary(),
            ExpiresAt: o.ExpiresAt
        ));
    }
}
