using VertexCommerce.Shared.Contracts.Pagination;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomers;

public sealed record GetCustomersQuery(string? SearchTerm) : PagedQuery, IQuery<PagedResult<CustomerAdminListItem>>;

public sealed record CustomerAdminListItem(
    Guid Id,
    Guid? UserId,
    string PhoneNumber,
    string FirstName,
    string LastName,
    DateTime CreatedAt
);
