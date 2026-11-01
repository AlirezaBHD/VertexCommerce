using VertexCommerce.Modules.Customers.Domain.Entities;

namespace VertexCommerce.Modules.Customers.Features.Customers.Queries.GetCustomer;

public sealed record AddressSummaryResponse(
    Guid Id,
    string Province,
    string City,
    string PostalAddress,
    string PostalCode,
    decimal Latitude,
    decimal Longitude,
    string? Label = null
)
{
    public static AddressSummaryResponse From(CustomerAddress address)
    {
        ArgumentNullException.ThrowIfNull(address);

        return new AddressSummaryResponse(
            Id: address.Id,
            Province: address.Address.Province.Value,
            City: address.Address.City.Value,
            PostalAddress: address.Address.PostalAddress.Value,
            PostalCode: address.Address.PostalCode.Value,
            Latitude: address.Address.Location.Latitude,
            Longitude: address.Address.Location.Longitude,
            Label: address.Label?.Value);
    }
}