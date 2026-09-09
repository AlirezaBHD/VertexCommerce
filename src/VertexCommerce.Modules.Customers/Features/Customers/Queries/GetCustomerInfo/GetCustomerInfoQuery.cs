using VertexCommerce.Shared.Contracts.Customers;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomerInfo;

public sealed record GetCustomerInfoQuery(Guid CustomerId) : IQuery<CustomerInfoDto?>;
