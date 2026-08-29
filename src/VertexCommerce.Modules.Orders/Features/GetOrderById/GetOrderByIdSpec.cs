using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Orders.Features.GetOrderById;

public sealed class GetOrderByIdSpec : BaseSpecification<Order, GetOrderByIdResponse>
{
    public GetOrderByIdSpec(Guid id)
    {
        Where(o => o.Id == id);
        
        Select(o => new GetOrderByIdResponse(
            Id: o.Id,
            CustomerId: o.CustomerId,
            CustomerName: null,
            CustomerPhoneNumber: o.CustomerPhoneNumber,
            OrderNumber: o.OrderNumber,
            Status: o.Status.ToString(),
            PaymentStatus: o.PaymentStatus.ToString(),
            SubTotal: o.SubTotal.ToString(),
            TotalAmount: o.TotalAmount.ToString(),
            ReceiptImagePath: o.ReceiptImagePath,
            TrackingNumber: o.TrackingNumber,
            ShippingAddress: o.ShippingAddress.ToStringSummary(),
            CancellationReason: o.CancellationReason,
            CreatedAt: o.CreatedAt,
            UpdatedAt: o.UpdatedAt,
            ConfirmedAt: o.ConfirmedAt,
            ProcessingAt: o.ProcessingAt,
            ShippedAt: o.ShippedAt,
            DeliveredAt: o.DeliveredAt,
            CancelledAt: o.CancelledAt,
            ExpiresAt: o.ExpiresAt,
            Items: o.Items.Select(i => new GetOrderByIdOrderItemResponse(
                Id:i.Id,
                ProductId:i.ProductId,
                VariantId:i.VariantId,
                ProductName:i.ProductName,
                ProductSku:i.ProductSku,
                UnitPrice:i.UnitPrice,
                Quantity:i.Quantity,
                TotalPrice:i.TotalPrice
            )).ToList()
        ));
    }
}
