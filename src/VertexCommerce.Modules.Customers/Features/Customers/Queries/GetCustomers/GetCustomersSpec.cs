using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomers;

public sealed class GetCustomersSpec : BaseSpecification<Customer, CustomerAdminListItem>
{
    public GetCustomersSpec(string? searchTerm, string? sortBy = null, bool sortDescending = true)
    {
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim();
            Where(c => c.PhoneNumber.Contains(term) ||
                       c.FirstName.Contains(term) ||
                       c.LastName.Contains(term));
        }

        switch (sortBy?.ToLowerInvariant())
        {
            case "firstname":
                if (sortDescending) OrderByDesc(c => c.FirstName);
                else OrderByAsc(c => c.FirstName);
                break;
            case "lastname":
                if (sortDescending) OrderByDesc(c => c.LastName);
                else OrderByAsc(c => c.LastName);
                break;
            case "phonenumber":
                if (sortDescending) OrderByDesc(c => c.PhoneNumber);
                else OrderByAsc(c => c.PhoneNumber);
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
            PhoneNumber: c.PhoneNumber,
            FirstName: c.FirstName,
            LastName: c.LastName,
            CreatedAt: c.CreatedAt
        ));
    }
}
