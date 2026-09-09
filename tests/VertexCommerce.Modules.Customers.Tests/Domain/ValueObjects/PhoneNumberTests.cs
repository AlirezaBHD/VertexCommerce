using FluentAssertions;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Shared.Exceptions;
using Xunit;

namespace VertexCommerce.Modules.Customers.Tests.Domain.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("09123456789")]
    [InlineData("09001112233")]
    public void Accepts_a_well_formed_mobile_number(string value) =>
        PhoneNumber.Create(value).Value.Should().Be(value);

    [Theory]
    [InlineData("0912345678", "too short")]
    [InlineData("091234567890", "too long")]
    [InlineData("08123456789", "wrong prefix")]
    [InlineData("0912345678a", "not all digits")]
    [InlineData("+989123456789", "international form is not accepted")]
    public void Rejects_a_malformed_number(string value, string because)
    {
        var act = () => PhoneNumber.Create(value);

        act.Should().Throw<DomainValidationException>(because)
            .Which.Field.Should().Be(nameof(PhoneNumber));
    }

    [Fact]
    public void Folds_persian_digits_into_ascii()
    {
        // Without this the value would pass a \d pattern yet never match an equality lookup.
        PhoneNumber.Create("۰۹۱۲۳۴۵۶۷۸۹").Value.Should().Be("09123456789");
    }

    [Fact]
    public void Folds_arabic_indic_digits_into_ascii() =>
        PhoneNumber.Create("٠٩١٢٣٤٥٦٧٨٩").Value.Should().Be("09123456789");

    [Fact]
    public void Surrounding_whitespace_is_ignored() =>
        PhoneNumber.Create("  09123456789  ").Value.Should().Be("09123456789");

    [Fact]
    public void A_number_typed_in_either_script_is_the_same_number() =>
        PhoneNumber.Create("۰۹۱۲۳۴۵۶۷۸۹").Should().Be(PhoneNumber.Create("09123456789"));
}
