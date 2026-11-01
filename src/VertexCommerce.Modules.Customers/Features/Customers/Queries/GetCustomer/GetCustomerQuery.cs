using VertexCommerce.Modules.Customers.Domain.Entities;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomer;

public sealed record GetCustomerQuery() : IQuery<CustomerResponse>;