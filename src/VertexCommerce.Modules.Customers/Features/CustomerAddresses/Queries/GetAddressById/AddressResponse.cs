using VertexCommerce.Modules.Customers.Domain.Entities;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Queries.GetAddressById;

public sealed record AddressResponse(
    Guid Id,
    Guid CustomerId,
    string Province,
    string City,
    string PostalAddress,
    string PostalCode,
    decimal Latitude,
    decimal Longitude,
    DateTime CreatedAt,
    string? Label = null
)
{
    /// <summary>
    /// Flattens a saved address into its wire representation. Defined once so the four routes that
    /// return an address cannot disagree about the shape.
    /// </summary>
    public static AddressResponse From(CustomerAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);

        return new AddressResponse(
            Id: address.Id,
            CustomerId: address.CustomerId,
            Province: address.Address.Province.Value,
            City: address.Address.City.Value,
            PostalAddress: address.Address.PostalAddress.Value,
            PostalCode: address.Address.PostalCode.Value,
            Latitude: address.Address.Location.Latitude,
            Longitude: address.Address.Location.Longitude,
            CreatedAt: address.CreatedAt,
            Label: address.Label?.Value);
    }
}
