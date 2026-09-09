using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Orders.Persistence;
using VertexCommerce.Shared.Contracts.Pagination;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Orders.Features.GetAllOrders;

public sealed class GetAllOrdersQueryHandler(OrdersDbContext dbContext)
    : IQueryHandler<GetAllOrdersQuery, PagedResult<AllOrdersResponse>>
{
    public async Task<Result<PagedResult<AllOrdersResponse>>> Handle(GetAllOrdersQuery query, CancellationToken ct)
    {
        var dbQuery = dbContext.Orders.AsNoTracking();

        if (query.CustomerId.HasValue)
        {
            dbQuery = dbQuery.Where(o => o.CustomerId == query.CustomerId.Value);
        }

        var count = await dbQuery.CountAsync(ct);

        var result = await dbQuery
            .OrderByDescending(o => o.UpdatedAt ?? o.CreatedAt)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(o => new AllOrdersResponse(
                o.Id,
                o.CustomerPhoneNumber.Value,
                o.OrderNumber.Value,
                o.Status.ToString(),
                o.PaymentStatus.ToString(),
                o.TotalAmount.ToString(),
                o.TrackingNumber == null ? null : o.TrackingNumber.Value.Value,
                o.CreatedAt,
                o.UpdatedAt,
                o.ExpiresAt
            ))
            .ToListAsync(ct);

        return Result.Success(new PagedResult<AllOrdersResponse>(
            Items: result,
            HasNextPage: count > query.Skip * query.Take,
            HasPreviousPage: query.Skip * query.Take - query.Take > 0,
            TotalCount: count
        ));
    }
}
