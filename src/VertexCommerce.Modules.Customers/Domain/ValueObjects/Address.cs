namespace VertexCommerce.Modules.Customers.Domain.ValueObjects;

/// <summary>
/// A complete deliverable address. Composed of typed parts so that a caller cannot transpose the
/// province with the city, and replaced as a whole rather than field by field.
/// </summary>
/// <remarks>
/// Deliberately excludes the user's label: two addresses describing the same place are the same
/// address regardless of what either was nicknamed. The label belongs to the saved entry, so it
/// lives on <see cref="Entities.CustomerAddress"/>.
/// </remarks>
public readonly record struct Address
{
    public Province Province { get; init; }
    public City City { get; init; }
    public PostalAddress PostalAddress { get; init; }
    public PostalCode PostalCode { get; init; }
    public GeoLocation Location { get; init; }

    public static Address Create(
        Province province,
        City city,
        PostalAddress postalAddress,
        PostalCode postalCode,
        GeoLocation location)
    {
        return new Address
        {
            Province = province,
            City = city,
            PostalAddress = postalAddress,
            PostalCode = postalCode,
            Location = location
        };
    }

    public override string ToString() => $"{Province.Value}، {City.Value}، {PostalAddress.Value} — {PostalCode.Value}";
}
