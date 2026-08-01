using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Shared.Contracts.Pagination;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomers;

internal sealed class GetCustomersQueryHandler(ICustomerRepository customerRepository)
    : IQueryHandler<GetCustomersQuery, PagedResult<CustomerAdminListItem>>
{
    public async Task<Result<PagedResult<CustomerAdminListItem>>> Handle(
        GetCustomersQuery query,
        CancellationToken ct)
    {
        var spec = new GetCustomersSpec(query.SearchTerm);

        var result = await customerRepository.GetPaginatedAsync(
            spec, skip: query.Skip, take: query.Take, ct);

        return Result.Success(result);
    }
}
