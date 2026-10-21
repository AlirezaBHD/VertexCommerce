using VertexCommerce.Shared.Exceptions;

namespace VertexCommerce.Modules.Orders.Domain.ValueObjects;


public readonly record struct GeoLocation
{
    private const decimal MinLatitude = -90m;
    private const decimal MaxLatitude = 90m;
    private const decimal MinLongitude = -180m;
    private const decimal MaxLongitude = 180m;

    public decimal Latitude { get; init; }
    public decimal Longitude { get; init; }

    public static GeoLocation Create(decimal latitude, decimal longitude)
    {
        if (latitude is < MinLatitude or > MaxLatitude)
        {
            throw new DomainValidationException(
                nameof(Latitude), $"Latitude must be between {MinLatitude} and {MaxLatitude}.");
        }

        if (longitude is < MinLongitude or > MaxLongitude)
        {
            throw new DomainValidationException(
                nameof(Longitude), $"Longitude must be between {MinLongitude} and {MaxLongitude}.");
        }

        return new GeoLocation { Latitude = latitude, Longitude = longitude };
    }

    public override string ToString() => $"{Latitude},{Longitude}";
}

