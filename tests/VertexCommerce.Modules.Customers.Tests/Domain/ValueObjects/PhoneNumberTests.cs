using FluentAssertions;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Shared.Exceptions;
using Xunit;

namespace VertexCommerce.Modules.Customers.Tests.Domain.ValueObjects;

public class PhoneNumberTests
{
    [Theory]
    [InlineData("+989123456789", "+989123456789")]
    [InlineData("+12025550123", "+12025550123")]
    [InlineData("+447911123456", "+447911123456")]
    [InlineData("09123456789", "+989123456789")]
    [InlineData("09001112233", "+989001112233")]
    [InlineData("989123456789", "+989123456789")]
    [InlineData("00989123456789", "+989123456789")]
    public void Accepts_and_normalizes_valid_phone_numbers(string input, string expected) =>
        PhoneNumber.Create(input).Value.Should().Be(expected);

    [Theory]
    [InlineData("0912345678", "too short")]
    [InlineData("091234567890", "too long")]
    [InlineData("08123456789", "wrong prefix")]
    [InlineData("0912345678a", "not all digits")]
    [InlineData("+1", "too short for international")]
    [InlineData("invalid", "not a phone number")]
    public void Rejects_a_malformed_number(string value, string because)
    {
        var act = () => PhoneNumber.Create(value);

        act.Should().Throw<DomainValidationException>(because)
            .Which.Field.Should().Be(nameof(PhoneNumber));
    }

    [Fact]
    public void Folds_persian_digits_into_ascii()
    {
        PhoneNumber.Create("۰۹۱۲۳۴۵۶۷۸۹").Value.Should().Be("+989123456789");
    }

    [Fact]
    public void Folds_arabic_indic_digits_into_ascii() =>
        PhoneNumber.Create("٠٩١٢٣٤٥٦٧٨٩").Value.Should().Be("+989123456789");

    [Fact]
    public void Surrounding_whitespace_is_ignored() =>
        PhoneNumber.Create("  09123456789  ").Value.Should().Be("+989123456789");

    [Fact]
    public void A_number_typed_in_either_script_is_the_same_number() =>
        PhoneNumber.Create("۰۹۱۲۳۴۵۶۷۸۹").Should().Be(PhoneNumber.Create("09123456789"));

    [Fact]
    public void A_local_and_international_form_produce_same_value() =>
        PhoneNumber.Create("09123456789").Should().Be(PhoneNumber.Create("+989123456789"));
}
