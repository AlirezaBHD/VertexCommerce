namespace VertexCommerce.Modules.Customers.Features.CustomerAddresses;

/// <summary>
/// The address fields carried by every command that creates or edits an address.
/// </summary>
/// <remarks>
/// Four endpoints accept the same address payload. Naming that shape once lets the validation rules
/// and the domain mapping be written once too, so the customer-facing and admin-facing routes cannot
/// drift apart.
/// </remarks>
public interface IAddressInput
{
    string Province { get; }
    string City { get; }
    string PostalAddress { get; }
    string PostalCode { get; }
    decimal Latitude { get; }
    decimal Longitude { get; }
    string? Label { get; }
}
