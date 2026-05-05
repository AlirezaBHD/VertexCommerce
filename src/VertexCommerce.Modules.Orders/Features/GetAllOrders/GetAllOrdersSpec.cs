using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Orders.Features.GetAllOrders;

public sealed class GetAllOrdersSpec : BaseSpecification<Order, AllOrdersResponse>
{
    public GetAllOrdersSpec()
    {
        OrderByDesc(o => o.UpdatedAt ?? o.CreatedAt);

        Select(o => new AllOrdersResponse(
            Id: o.Id,
            CustomerPhoneNumber: o.CustomerPhoneNumber,
            OrderNumber: o.OrderNumber,
            Status: o.Status.ToString(),
            PaymentStatus: o.PaymentStatus.ToString(),
            TotalAmount: o.TotalAmount.ToString(),
            TrackingNumber: o.TrackingNumber,
            CreatedAt: o.CreatedAt,
            UpdatedAt: o.UpdatedAt
        ));
    }
}
