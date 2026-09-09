using FluentAssertions;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Shared.Exceptions;
using Xunit;

namespace VertexCommerce.Modules.Customers.Tests.Domain.ValueObjects;

public class PostalCodeTests
{
    [Fact]
    public void Accepts_exactly_ten_digits() =>
        PostalCode.Create("1234567890").Value.Should().Be("1234567890");

    [Theory]
    [InlineData("123456789", "nine digits")]
    [InlineData("12345678901", "eleven digits")]
    [InlineData("12345-6789", "contains a separator")]
    [InlineData("abcdefghij", "letters")]
    [InlineData("", "blank")]
    public void Rejects_anything_that_is_not_ten_digits(string value, string because)
    {
        var act = () => PostalCode.Create(value);

        act.Should().Throw<DomainValidationException>(because)
            .Which.Field.Should().Be(nameof(PostalCode));
    }

    [Fact]
    public void Folds_persian_digits_into_ascii() =>
        PostalCode.Create("۱۲۳۴۵۶۷۸۹۰").Value.Should().Be("1234567890");
}
