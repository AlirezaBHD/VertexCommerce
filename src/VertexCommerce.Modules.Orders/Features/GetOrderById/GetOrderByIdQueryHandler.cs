using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.CQRS;
using VertexCommerce.Modules.Orders.Domain.Errors;

namespace VertexCommerce.Modules.Orders.Features.GetOrderById;

internal sealed class GetOrderByIdQueryHandler(
    OrdersDbContext dbContext,
    ICustomerService customerService)
    : IQueryHandler<GetOrderByIdQuery, GetOrderByIdResponse>
{
    public async Task<Result<GetOrderByIdResponse>> Handle(GetOrderByIdQuery query, CancellationToken ct)
    {
        var orderResponse = await dbContext.Orders
            .AsNoTracking()
            .Where(o => o.Id == query.OrderId)
            .Select(o => new GetOrderByIdResponse(
                o.Id,
                o.CustomerId,
                null,
                o.CustomerPhoneNumber.Value,
                o.OrderNumber.Value,
                o.Status.ToString(),
                o.PaymentStatus.ToString(),
                o.SubTotal.ToString(),
                o.TotalAmount.ToString(),
                o.ReceiptImagePath,
                o.TrackingNumber == null ? null : o.TrackingNumber.Value.Value,
                o.ShippingAddress.ToString(),
                o.CancellationReason,
                o.CreatedAt,
                o.UpdatedAt,
                o.ConfirmedAt,
                o.ProcessingAt,
                o.ShippedAt,
                o.DeliveredAt,
                o.CancelledAt,
                o.ExpiresAt,
                o.Items.Select(i => new GetOrderByIdOrderItemResponse(
                    i.Id,
                    i.ProductId,
                    i.VariantId,
                    i.ProductName,
                    i.ProductSku,
                    i.UnitPrice,
                    i.Quantity,
                    i.TotalPrice
                )).ToList()
            ))
            .FirstOrDefaultAsync(ct);

        if (orderResponse is null)
        {
            return Result.Failure<GetOrderByIdResponse>(OrderErrors.NotFound(query.OrderId));
        }

        var customerInfo = await customerService.GetCustomerInfo(orderResponse.CustomerId, ct);

        var finalResponse = orderResponse with
        {
            CustomerName = customerInfo is not null ? $"{customerInfo.FirstName} {customerInfo.LastName}" : "Unknown"
        };

        return Result.Success(finalResponse);
    }
}
