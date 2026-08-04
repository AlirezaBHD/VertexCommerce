using VertexCommerce.Shared.Contracts.Pagination;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomers;

public sealed record GetCustomersQuery(
    string? SearchTerm,
    string? SortBy = null,
    bool SortDescending = true
) : PagedQuery, IQuery<PagedResult<CustomerAdminListItem>>;

public sealed record CustomerAdminListItem(
    Guid Id,
    Guid? UserId,
    string PhoneNumber,
    string FirstName,
    string LastName,
    DateTime CreatedAt
);
