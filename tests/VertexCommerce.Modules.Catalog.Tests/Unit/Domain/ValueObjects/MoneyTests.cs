using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Shared.Exceptions;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Domain.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateMoney()
    {
        // Arrange
        var currency = Currency.Create("USD");

        // Act
        var money = Money.Create(100.50m, currency);

        // Assert
        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be(currency);
        money.Currency.Value.Should().Be("USD");
    }

    [Fact]
    public void Create_WithZeroAmount_ShouldSucceed()
    {
        // Act
        var money = Money.Create(0m, Currency.Create("IRR"));

        // Assert
        money.Amount.Should().Be(0m);
        money.Currency.Value.Should().Be("IRR");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowException()
    {
        // Act
        var act = () => Money.Create(-10m, Currency.Create("USD"));

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100m, Currency.Create("USD"));
        var money2 = Money.Create(100m, Currency.Create("USD"));

        // Assert
        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentAmount_ShouldNotBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100m, Currency.Create("USD"));
        var money2 = Money.Create(50m, Currency.Create("USD"));

        // Assert
        money1.Should().NotBe(money2);
        (money1 == money2).Should().BeFalse();
    }

    [Fact]
    public void Equals_WithDifferentCurrency_ShouldNotBeEqual()
    {
        // Arrange
        var money1 = Money.Create(100m, Currency.Create("USD"));
        var money2 = Money.Create(100m, Currency.Create("EUR"));

        // Assert
        money1.Should().NotBe(money2);
        (money1 == money2).Should().BeFalse();
    }

    [Fact]
    public void Currency_Create_WithValidCode_ShouldSetNormalizedValue()
    {
        // Act
        var currency = Currency.Create("  USD  ");

        // Assert
        currency.Value.Should().Be("USD");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Currency_Create_WithEmptyValue_ShouldThrowDomainValidationException(string? value)
    {
        // Act
        var act = () => Currency.Create(value);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*required*");
    }

    [Fact]
    public void Currency_Create_WithLengthGreaterThan3_ShouldThrowDomainValidationException()
    {
        // Act
        var act = () => Currency.Create("USDT");

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*must not exceed 3 characters*");
    }

    [Fact]
    public void Currency_Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var c1 = Currency.Create("USD");
        var c2 = Currency.Create("USD");

        // Assert
        c1.Should().Be(c2);
        (c1 == c2).Should().BeTrue();
    }
}