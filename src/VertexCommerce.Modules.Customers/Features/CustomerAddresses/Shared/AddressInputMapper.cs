using VertexCommerce.Modules.Customers.Domain.ValueObjects;

namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses.Shared;

/// <summary>
/// Translates primitive command input into the domain's value objects. This is the single place
/// where untyped strings become typed domain concepts, so no handler has to know the construction
/// order and none can transpose two fields.
/// </summary>
internal static class AddressInputMapper
{
    public static Address ToAddress(this IAddressInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return Address.Create(
            province: Province.Create(input.Province),
            city: City.Create(input.City),
            postalAddress: PostalAddress.Create(input.PostalAddress),
            postalCode: PostalCode.Create(input.PostalCode),
            location: GeoLocation.Create(input.Latitude, input.Longitude));
    }

    public static AddressLabel? ToLabel(this IAddressInput input)
    {
        ArgumentNullException.ThrowIfNull(input);

        return AddressLabel.CreateOrNull(input.Label);
    }
}
