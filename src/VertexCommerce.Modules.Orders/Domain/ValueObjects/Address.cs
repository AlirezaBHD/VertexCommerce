using VertexCommerce.Shared.Domain.Schema;

namespace VertexCommerce.Modules.Orders.Domain.ValueObjects;

public readonly record struct Address
{
    public static StringFieldSchema ProvinceSchema { get; } = new(maxLength: 100);
    public static StringFieldSchema CitySchema { get; } = new(maxLength: 100);
    public static StringFieldSchema PostalAddressSchema { get; } = new(maxLength: 500);
    public static StringFieldSchema PostalCodeSchema { get; } = new(maxLength: 20);

    public string Province { get; init; }
    public string City { get; init; }
    public string PostalAddress { get; init; }
    public string PostalCode { get; init; }
    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }
    public string? Label { get; init; }

    public static Address Create(string? province, string? city, string? postalAddress, string? postalCode, decimal latitude,
        decimal longitude, string? label)
    {
        return new Address
        {
            Province = StringFieldGuard.Apply(province, ProvinceSchema, nameof(Province)),
            City = StringFieldGuard.Apply(city, CitySchema, nameof(City)),
            PostalAddress = StringFieldGuard.Apply(postalAddress, PostalAddressSchema, nameof(PostalAddress)),
            PostalCode = StringFieldGuard.Apply(postalCode, PostalCodeSchema, nameof(PostalCode)),
            Latitude = latitude,
            Longitude = longitude,
            Label = label?.Trim()
        };
    }

    public override string ToString() => $"{Province} - {City} - {PostalAddress} - {PostalCode}";
    public string ToStringSummary() => 
        $"{Province} - {City} - {(PostalAddress.Length > 10 ? PostalAddress[..10] + "..." : PostalAddress)}";
}
