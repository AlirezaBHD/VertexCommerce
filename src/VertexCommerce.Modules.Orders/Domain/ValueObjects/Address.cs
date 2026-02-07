using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Orders.Domain.ValueObjects;

public sealed class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string State { get; private set; }
    public string Country { get; private set; }
    public string ZipCode { get; private set; }

    private Address()
    {
        Street = string.Empty;
        City = string.Empty;
        State = string.Empty;
        Country = string.Empty;
        ZipCode = string.Empty;
    }

    private Address(string street, string city, string state, string country, string zipCode)
    {
        Street = street;
        City = city;
        State = state;
        Country = country;
        ZipCode = zipCode;
    }

    public static Address Create(string street, string city, string state, string country, string zipCode)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty.", nameof(street));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty.", nameof(city));

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be empty.", nameof(country));

        if (string.IsNullOrWhiteSpace(zipCode))
            throw new ArgumentException("ZipCode cannot be empty.", nameof(zipCode));

        return new Address(
            street.Trim(),
            city.Trim(),
            state?.Trim() ?? string.Empty,
            country.Trim(),
            zipCode.Trim()
        );
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return Country;
        yield return ZipCode;
    }

    public override string ToString() => $"{Street}, {City}, {State} {ZipCode}, {Country}";
}
