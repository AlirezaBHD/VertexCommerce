using FluentAssertions;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Domain.ValueObjects;

public class ColorHexTests
{
    [Theory]
    [InlineData("#000000")]
    [InlineData("#FFFFFF")]
    [InlineData("#a1b2c3")]
    public void Create_WithSixDigitHex_ShouldCreateColorHex(string value)
    {
        var colorHex = ColorHex.Create(value);

        colorHex.Value.Should().Be(value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("#FFF")]
    [InlineData("000000")]
    [InlineData("#GG0000")]
    [InlineData("#00000000")]
    public void Create_WithInvalidHex_ShouldThrowArgumentException(string value)
    {
        var act = () => ColorHex.Create(value);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void CreateOrNull_WithMissingValue_ShouldReturnNull()
    {
        ColorHex.CreateOrNull(null).Should().BeNull();
        ColorHex.CreateOrNull(" ").Should().BeNull();
    }
}
