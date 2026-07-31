using VertexCommerce.Modules.Customers.Domain.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.SearchCustomers;

internal sealed class SearchCustomersQueryHandler(ICustomerRepository customerRepository)
    : IQueryHandler<SearchCustomersQuery, IReadOnlyList<CustomerSearchItem>>
{
    public async Task<Result<IReadOnlyList<CustomerSearchItem>>> Handle(
        SearchCustomersQuery query,
        CancellationToken ct)
    {
        var customers = await customerRepository.SearchAsync(query.SearchTerm, query.Limit, ct);

        var items = customers
            .Select(c => new CustomerSearchItem(
                Id: c.Id,
                UserId: c.UserId,
                PhoneNumber: c.PhoneNumber,
                FirstName: c.FirstName,
                LastName: c.LastName))
            .ToList();

        return Result.Success((IReadOnlyList<CustomerSearchItem>)items);
    }
}
