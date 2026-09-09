using FluentAssertions;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Shared.Exceptions;
using Xunit;

namespace VertexCommerce.Modules.Customers.Tests.Domain.ValueObjects;

public class GeoLocationTests
{
    [Fact]
    public void Accepts_a_point_inside_the_valid_ranges()
    {
        var tehran = GeoLocation.Create(35.6892m, 51.3890m);

        tehran.Latitude.Should().Be(35.6892m);
        tehran.Longitude.Should().Be(51.3890m);
    }

    [Theory]
    [InlineData(-90, 0)]
    [InlineData(90, 0)]
    [InlineData(0, -180)]
    [InlineData(0, 180)]
    public void Accepts_the_boundaries(decimal latitude, decimal longitude)
    {
        var act = () => GeoLocation.Create(latitude, longitude);

        act.Should().NotThrow();
    }

    [Theory]
    [InlineData(-90.1, 0)]
    [InlineData(90.1, 0)]
    public void Rejects_an_out_of_range_latitude(decimal latitude, decimal longitude)
    {
        var act = () => GeoLocation.Create(latitude, longitude);

        act.Should().Throw<DomainValidationException>()
            .Which.Field.Should().Be("Latitude");
    }

    [Theory]
    [InlineData(0, -180.1)]
    [InlineData(0, 180.1)]
    public void Rejects_an_out_of_range_longitude(decimal latitude, decimal longitude)
    {
        var act = () => GeoLocation.Create(latitude, longitude);

        act.Should().Throw<DomainValidationException>()
            .Which.Field.Should().Be("Longitude");
    }

    [Fact]
    public void Equality_is_by_coordinate() =>
        GeoLocation.Create(35.6892m, 51.3890m)
            .Should().Be(GeoLocation.Create(35.6892m, 51.3890m));
}
