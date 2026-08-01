using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomers;

public sealed class GetCustomersSpec : BaseSpecification<Customer, CustomerAdminListItem>
{
    public GetCustomersSpec(string? searchTerm)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            Where(c => c.PhoneNumber.Contains(term) ||
                       c.FirstName.Contains(term) ||
                       c.LastName.Contains(term));
        }

        OrderByDesc(c => c.CreatedAt);

        Select(c => new CustomerAdminListItem(
            Id: c.Id,
            UserId: c.UserId,
            PhoneNumber: c.PhoneNumber,
            FirstName: c.FirstName,
            LastName: c.LastName,
            CreatedAt: c.CreatedAt
        ));
    }
}
