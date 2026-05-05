using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Queries.GetAddressById;

public sealed record GetAddressByIdQuery(Guid AddressId) : IQuery<AddressResponse>;



