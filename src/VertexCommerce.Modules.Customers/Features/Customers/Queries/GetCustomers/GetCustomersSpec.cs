using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomers;

public sealed class GetCustomersSpec : BaseSpecification<Customer, CustomerAdminListItem>
{
    public GetCustomersSpec(string? searchTerm, string? sortBy = null, bool sortDescending = true)
    {
        // Value objects are mapped as complex types, so .Value is a real column and these
        // predicates still translate to SQL.
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            Where(c => c.PhoneNumber.Value.Contains(term) ||
                       c.FirstName.Value.Contains(term) ||
                       c.LastName.Value.Contains(term));
        }

        switch (sortBy?.ToLowerInvariant())
        {
            case "firstname":
                if (sortDescending) OrderByDesc(c => c.FirstName.Value);
                else OrderByAsc(c => c.FirstName.Value);
                break;
            case "lastname":
                if (sortDescending) OrderByDesc(c => c.LastName.Value);
                else OrderByAsc(c => c.LastName.Value);
                break;
            case "phonenumber":
                if (sortDescending) OrderByDesc(c => c.PhoneNumber.Value);
                else OrderByAsc(c => c.PhoneNumber.Value);
                break;
            case "createdat":
            default:
                if (sortDescending) OrderByDesc(c => c.CreatedAt);
                else OrderByAsc(c => c.CreatedAt);
                break;
        }

        Select(c => new CustomerAdminListItem(
            Id: c.Id,
            UserId: c.UserId,
            PhoneNumber: c.PhoneNumber.Value,
            FirstName: c.FirstName.Value,
            LastName: c.LastName.Value,
            CreatedAt: c.CreatedAt
        ));
    }
}
