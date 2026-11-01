namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomers;

public sealed record CustomerAdminListItem(
    Guid Id,
    Guid? UserId,
    string PhoneNumber,
    string FirstName,
    string LastName,
    DateTime CreatedAt
);