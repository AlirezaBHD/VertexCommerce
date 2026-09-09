using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.Contracts.Identity;
using VertexCommerce.Shared.Contracts.Pagination;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.GetMyOrders;

public sealed class GetMyOrdersQueryHandler(
    OrdersDbContext dbContext,
    ICurrentUser currentUser,
    ICustomerResolver customerResolver)
    : IQueryHandler<GetMyOrdersQuery, PagedResult<MyOrdersResponse>>
{
    public async Task<Result<PagedResult<MyOrdersResponse>>> Handle(GetMyOrdersQuery query, CancellationToken ct)
    {
        var customerId = await customerResolver.GetCustomerIdByUserIdAsync(currentUser.UserId, ct);

        var dbQuery = dbContext.Orders
            .AsNoTracking()
            .Where(o => o.CustomerId == customerId);

        var count = await dbQuery.CountAsync(ct);

        var result = await dbQuery
            .OrderByDescending(o => o.CreatedAt)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(o => new MyOrdersResponse(
                o.Id,
                o.OrderNumber.Value,
                o.Status.ToString(),
                o.PaymentStatus.ToString(),
                o.SubTotal.ToString(),
                o.TotalAmount.ToString(),
                o.TrackingNumber == null ? null : o.TrackingNumber.Value.Value,
                o.ShippingAddress.ToString(),
                o.ExpiresAt
            ))
            .ToListAsync(ct);

        return Result.Success(new PagedResult<MyOrdersResponse>(
            Items: result,
            HasNextPage: count > query.Skip * query.Take,
            HasPreviousPage: query.Skip * query.Take - query.Take > 0,
            TotalCount: count
        ));
    }
}
