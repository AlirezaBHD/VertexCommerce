using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Orders.Domain.ValueObjects;

public sealed class Address : ValueObject
{
    public string Province { get; private set; } = default!;
    public string City { get; private set; } = default!;
    public string PostalAddress { get; private set; } = default!;
    public string PostalCode { get; private set; } = default!;
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    public string? Label { get; private set; }

    private Address()
    {
        Province = string.Empty;
        City = string.Empty;
        PostalAddress = string.Empty;
        PostalCode = string.Empty;
    }

    private Address(string province, string city, string postalAddress, string postalCode, decimal latitude,
        decimal longitude, string? label)
    {
        Province = province;
        City = city;
        PostalAddress = postalAddress;
        PostalCode = postalCode;
        Latitude = latitude;
        Longitude = longitude;
        Label = label;
    }

    public static Address Create(string province, string city, string postalAddress, string postalCode, decimal latitude,
        decimal longitude, string? label)
    {
        if (string.IsNullOrWhiteSpace(province))
            throw new ArgumentException("province cannot be empty.", nameof(province));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty.", nameof(city));

        if (string.IsNullOrWhiteSpace(postalAddress))
            throw new ArgumentException("postalAddress cannot be empty.", nameof(postalAddress));

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("postalCode cannot be empty.", nameof(postalCode));

        return new Address(
            province: province.Trim(),
            city: city.Trim(),
            postalAddress: postalAddress.Trim(),
            postalCode: postalCode.Trim(),
            latitude: latitude,
            longitude: longitude,
            label: label?.Trim());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Province;
        yield return City;
        yield return PostalAddress;
        yield return PostalCode;
    }

    public override string ToString() => $"{Province}، {City}، {PostalAddress} — {PostalCode}";
    public string ToStringSummary() => 
        $"{Province}، {City}، {(PostalAddress.Length > 10 ? PostalAddress[..10] + "..." : PostalAddress)}";
}
