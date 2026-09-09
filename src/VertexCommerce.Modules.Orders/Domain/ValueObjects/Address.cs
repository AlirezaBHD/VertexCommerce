namespace VertexCommerce.Modules.Orders.Domain.ValueObjects;

public readonly record struct Address
{
    public Province Province { get; init; }
    public City City { get; init; }
    public PostalAddress PostalAddress { get; init; }
    public PostalCode PostalCode { get; init; }
    public GeoLocation Location { get; init; }
    public AddressLabel? Label { get; init; }

    public static Address Create(
        Province province,
        City city,
        PostalAddress postalAddress,
        PostalCode postalCode,
        GeoLocation location,
        AddressLabel? label = null)
    {
        return new Address
        {
            Province = province,
            City = city,
            PostalAddress = postalAddress,
            PostalCode = postalCode,
            Location = location,
            Label = label
        };
    }

    public override string ToString()
    {
        return Province.Value + " - " + City.Value + " - " + PostalAddress.Value + " - " + PostalCode.Value;
    }

    public string ToStringSummary()
    {
        string pAddress = PostalAddress.Value;
        if (pAddress.Length > 10)
        {
            pAddress = pAddress.Substring(0, 10) + "...";
        }
        return Province.Value + " - " + City.Value + " - " + pAddress;
    }
}
