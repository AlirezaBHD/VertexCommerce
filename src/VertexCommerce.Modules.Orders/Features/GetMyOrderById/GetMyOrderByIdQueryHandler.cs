using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Modules.Orders.Domain.Errors;

namespace VertexCommerce.Modules.Orders.Features.GetMyOrderById;

public sealed class GetMyOrderByIdQueryHandler(
    OrdersDbContext dbContext,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver)
    : IQueryHandler<GetMyOrderByIdQuery, MyOrderResponse>
{
    public async Task<Result<MyOrderResponse>> Handle(GetMyOrderByIdQuery query, CancellationToken ct)
    {
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(currentUser.UserId, ct);

        var order = await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId && o.Id == query.OrderId)
            .Select(o => new MyOrderResponse(
                o.Id,
                o.OrderNumber.Value,
                o.CustomerPhoneNumber.Value,
                o.Status.ToString(),
                o.PaymentStatus.ToString(),
                o.ReceiptImagePath,
                o.TransactionReference,
                o.ShippingAddress.ToString(),
                o.BillingAddress.ToString(),
                o.SubTotal.Amount,
                o.ShippingCost.Amount,
                o.Tax.Amount,
                o.TotalAmount.Amount,
                o.TotalAmount.Currency,
                o.Notes,
                o.CancellationReason,
                o.TrackingNumber == null ? null : o.TrackingNumber.Value.Value,
                o.CreatedAt,
                o.ConfirmedAt,
                o.ProcessingAt,
                o.ShippedAt,
                o.DeliveredAt,
                o.CancelledAt,
                o.ExpiresAt,
                o.Items.Select(i => new OrderItemResponse(
                    i.Id,
                    i.ProductId,
                    i.VariantId,
                    i.ProductName,
                    i.ProductSku,
                    i.UnitPrice.Amount,
                    i.Quantity,
                    i.TotalPrice.Amount
                )).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        if (order is null)
        {
            return Result.Failure<MyOrderResponse>(OrderErrors.NotFound(query.OrderId));
        }

        return Result.Success(order);
    }
}
