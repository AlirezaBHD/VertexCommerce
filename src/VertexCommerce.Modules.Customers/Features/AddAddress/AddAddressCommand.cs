using VertexCommerce.Modules.Customers.Features.GetCustomer;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Customers.Features.AddAddress;

public sealed record AddAddressCommand(
    Guid UserId,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    string? Label
) : ICommand<AddressResponse>;
