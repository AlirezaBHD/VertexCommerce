using FluentAssertions;
using VertexCommerce.Modules.Customers.Domain.ValueObjects;
using VertexCommerce.Shared.Domain;
using VertexCommerce.Shared.Domain.Schema;
using VertexCommerce.Shared.Exceptions;
using Xunit;

namespace VertexCommerce.Modules.Customers.Tests.Domain.ValueObjects;

/// <summary>
/// The behaviour that was unreachable while the constraints lived in an attribute: the type itself
/// now refuses to exist in an invalid state.
/// </summary>
public class StringValueObjectGuardTests
{
    private static readonly Action<string?>[] RequiredFactories =
    [
        value => FirstName.Create(value),
        value => LastName.Create(value),
        value => Province.Create(value),
        value => City.Create(value),
        value => PostalAddress.Create(value)
    ];

    public static TheoryData<string> Blanks => new() { "", "   ", "\t" };

    [Theory]
    [MemberData(nameof(Blanks))]
    public void Required_concepts_reject_blank_input(string blank)
    {
        foreach (var create in RequiredFactories)
        {
            var act = () => create(blank);
            act.Should().Throw<DomainValidationException>();
        }
    }

    [Fact]
    public void Required_concepts_reject_null()
    {
        foreach (var create in RequiredFactories)
        {
            var act = () => create(null);
            act.Should().Throw<DomainValidationException>();
        }
    }

    [Fact]
    public void Values_are_trimmed()
    {
        FirstName.Create("  علی  ").Value.Should().Be("علی");
        LastName.Create("\tاحمدی\n").Value.Should().Be("احمدی");
        City.Create(" تهران ").Value.Should().Be("تهران");
    }

    [Fact]
    public void Values_longer_than_the_spec_are_rejected()
    {
        var atLimit = new string('ا', FirstName.Schema.MaxLength);
        var overLimit = new string('ا', FirstName.Schema.MaxLength + 1);

        FirstName.Create(atLimit).Value.Should().HaveLength(FirstName.Schema.MaxLength);

        var act = () => FirstName.Create(overLimit);
        act.Should().Throw<DomainValidationException>()
            .Which.Field.Should().Be(nameof(FirstName));
    }

    [Fact]
    public void Equality_is_by_value_not_reference()
    {
        FirstName.Create("علی").Should().Be(FirstName.Create("علی"));
        FirstName.Create("علی").Should().NotBe(FirstName.Create("رضا"));

        // Trimming happens before comparison, so these are the same name.
        FirstName.Create(" علی ").Should().Be(FirstName.Create("علی"));
    }

    [Fact]
    public void Different_concepts_with_the_same_text_are_not_equal()
    {
        var province = Province.Create("تهران");
        var city = City.Create("تهران");

        province.Equals((object)city).Should().BeFalse(
            "distinct domain concepts must not compare equal just because their text matches");
    }
}
