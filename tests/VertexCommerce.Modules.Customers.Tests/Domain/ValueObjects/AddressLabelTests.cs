using FluentAssertions;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Shared.Exceptions;
using Xunit;

namespace VertexCommerce.Modules.Customers.Tests.Domain.ValueObjects;

public class AddressLabelTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Absence_is_modelled_as_null_rather_than_an_empty_label(string? value) =>
        AddressLabel.CreateOrNull(value).Should().BeNull();

    [Fact]
    public void A_label_that_exists_is_always_meaningful() =>
        AddressLabel.CreateOrNull("  خانه  ")!.Value.Value.Should().Be("خانه");

    [Fact]
    public void Rejects_a_label_longer_than_the_spec()
    {
        var act = () => AddressLabel.CreateOrNull(new string('ا', AddressLabel.Schema.MaxLength + 1));

        act.Should().Throw<DomainValidationException>()
            .Which.Field.Should().Be(nameof(AddressLabel));
    }
}
