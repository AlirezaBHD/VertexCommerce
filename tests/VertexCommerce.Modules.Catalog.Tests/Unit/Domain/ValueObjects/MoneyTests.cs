using FluentAssertions;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Domain.ValueObjects;

public class MoneyTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateMoney()
    {
        var money = Money.Create(100.50m, "USD");

        money.Amount.Should().Be(100.50m);
        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Create_WithNegativeAmount_ShouldThrowException()
    {
        var act = () => Money.Create(-10, "USD");

        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithInvalidCurrency_ShouldThrowException(string? currency)
    {
        var act = () => Money.Create(100, currency!);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Currency*");
    }

    [Fact]
    public void Create_ShouldNormalizeCurrencyToUpperCase()
    {
        var money = Money.Create(100, "usd");

        money.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_WithSameCurrency_ShouldReturnSum()
    {
        var money1 = Money.Create(100, "USD");
        var money2 = Money.Create(50, "USD");

        var result = money1.Add(money2);

        result.Amount.Should().Be(150);
        result.Currency.Should().Be("USD");
    }

    [Fact]
    public void Add_WithDifferentCurrency_ShouldThrowException()
    {
        var money1 = Money.Create(100, "USD");
        var money2 = Money.Create(50, "EUR");

        var act = () => money1.Add(money2);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*different currencies*");
    }

    [Fact]
    public void Subtract_WithSameCurrency_ShouldReturnDifference()
    {
        var money1 = Money.Create(100, "USD");
        var money2 = Money.Create(30, "USD");

        var result = money1.Subtract(money2);

        result.Amount.Should().Be(70);
    }

    [Fact]
    public void Subtract_WhenResultNegative_ShouldThrowException()
    {
        var money1 = Money.Create(30, "USD");
        var money2 = Money.Create(100, "USD");

        var act = () => money1.Subtract(money2);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void Multiply_WithPositiveQuantity_ShouldReturnProduct()
    {
        var money = Money.Create(25, "USD");

        var result = money.Multiply(4);

        result.Amount.Should().Be(100);
    }

    [Fact]
    public void Equals_WithSameValues_ShouldBeEqual()
    {
        var money1 = Money.Create(100, "USD");
        var money2 = Money.Create(100, "USD");

        money1.Should().Be(money2);
        (money1 == money2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValues_ShouldNotBeEqual()
    {
        var money1 = Money.Create(100, "USD");
        var money2 = Money.Create(100, "EUR");

        money1.Should().NotBe(money2);
    }

    [Fact]
    public void Zero_ShouldReturnZeroAmount()
    {
        var money = Money.Zero("EUR");

        money.Amount.Should().Be(0);
        money.Currency.Should().Be("EUR");
    }

    [Fact]
    public void ToString_ShouldReturnFormattedString()
    {
        var money = Money.Create(99.99m, "USD");

        var result = money.ToString();

        result.Should().Be("99.99 USD");
    }
}