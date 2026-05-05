using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Orders.Features.GetMyOrderById;

public sealed class GetMyOrderByIdSpec : BaseSpecification<Order, MyOrderResponse>
{
    public GetMyOrderByIdSpec(Guid customerId, Guid orderId)
    {
        Where(o => o.CustomerId == customerId && o.Id == orderId);
        Include(o => o.Items);
        Select(o => new MyOrderResponse(
            Id: o.Id,
            OrderNumber: o.OrderNumber,
            CustomerPhoneNumber: o.CustomerPhoneNumber,
            Status: o.Status.ToString(),
            PaymentStatus: o.PaymentStatus.ToString(),
            ReceiptImagePath: o.ReceiptImagePath,
            TransactionReference: o.TransactionReference,
            ShippingAddress: o.ShippingAddress.ToStringSummary(),
            BillingAddress: o.BillingAddress.ToStringSummary(),
            SubTotal: o.SubTotal.Amount,
            ShippingCost: o.ShippingCost.Amount,
            Tax: o.Tax.Amount,
            TotalAmount: o.TotalAmount.Amount,
            Currency: o.TotalAmount.Currency,
            Notes: o.Notes,
            CancellationReason: o.CancellationReason,
            TrackingNumber: o.TrackingNumber,
            CreatedAt: o.CreatedAt,
            ConfirmedAt: o.ConfirmedAt,
            ProcessingAt: o.ProcessingAt,
            ShippedAt: o.ShippedAt,
            DeliveredAt: o.DeliveredAt,
            CancelledAt: o.CancelledAt,
            Items: o.Items.Select(i => new OrderItemResponse(
                Id: i.Id,
                ProductId: i.ProductId,
                VariantId: i.VariantId,
                ProductName: i.ProductName,
                ProductSku: i.ProductSku,
                UnitPrice: i.UnitPrice.Amount,
                Quantity: i.Quantity,
                TotalPrice: i.TotalPrice.Amount
            )).ToList()
        ));
    }
}

