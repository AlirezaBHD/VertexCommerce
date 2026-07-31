using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.SearchCustomers;

public sealed record SearchCustomersQuery(
    string? SearchTerm,
    int Limit = 20
) : IQuery<IReadOnlyList<CustomerSearchItem>>;

public sealed record CustomerSearchItem(
    Guid Id,
    Guid? UserId,
    string PhoneNumber,
    string FirstName,
    string LastName
);
