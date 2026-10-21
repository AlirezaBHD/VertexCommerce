using FluentAssertions;
using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Modules.Orders.Domain.ValueObjects;
using Xunit;

namespace VertexCommerce.Modules.Orders.Tests.Domain;

public class ShippingSettingsTests
{
    [Fact]
    public void Create_WithValidData_ShouldSetPropertiesCorrectly()
    {
        // Act
        var settings = ShippingSettings.Create(
            cost: Money.Create(45000m, "IRR"),
            freeShippingThreshold: Money.Create(500000m, "IRR"),
            description: "ارسال سریع با پست پیشتاز",
            isActive: true);

        // Assert
        settings.Id.Should().NotBeEmpty();
        settings.Cost.Amount.Should().Be(45000m);
        settings.Cost.Currency.Should().Be("IRR");
        settings.FreeShippingThreshold.Amount.Should().Be(500000m);
        settings.Description.Should().Be("ارسال سریع با پست پیشتاز");
        settings.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithNegativeCost_ShouldThrowArgumentException()
    {
        // Act
        var act = () => ShippingSettings.Create(cost: Money.Create(-1000m));

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void CalculateShippingCost_WhenSubtotalExceedsFreeShippingThreshold_ShouldReturnZero()
    {
        // Arrange
        var settings = ShippingSettings.Create(
            cost: Money.Create(50000m, "USD"),
            freeShippingThreshold: Money.Create(400000m, "USD"));

        // Act
        var shippingCost = settings.CalculateShippingCost(subtotal: Money.Create(450000m, "USD"));

        // Assert
        shippingCost.Amount.Should().Be(0m);
        shippingCost.Currency.Should().Be("USD");
    }

    [Fact]
    public void CalculateShippingCost_WhenSubtotalBelowFreeShippingThreshold_ShouldReturnCost()
    {
        // Arrange
        var settings = ShippingSettings.Create(
            cost: Money.Create(50000m, "USD"),
            freeShippingThreshold: Money.Create(400000m, "USD"));

        // Act
        var shippingCost = settings.CalculateShippingCost(subtotal: Money.Create(350000m, "USD"));

        // Assert
        shippingCost.Amount.Should().Be(50000m);
        shippingCost.Currency.Should().Be("USD");
    }

    [Fact]
    public void CalculateShippingCost_WhenNotActive_ShouldReturnZero()
    {
        // Arrange
        var settings = ShippingSettings.Create(cost: Money.Create(50000m), isActive: false);

        // Act
        var shippingCost = settings.CalculateShippingCost(subtotal: Money.Create(100000m));

        // Assert
        shippingCost.Amount.Should().Be(0m);
    }

    [Fact]
    public void Update_ShouldUpdatePropertiesAndTimestamps()
    {
        // Arrange
        var settings = ShippingSettings.Create(cost: Money.Create(30000m));

        // Act
        settings.Update(
            cost: Money.Create(60000m),
            freeShippingThreshold: Money.Create(600000m),
            description: "بروزرسانی تعرفه",
            isActive: true);

        // Assert
        settings.Cost.Amount.Should().Be(60000m);
        settings.FreeShippingThreshold.Amount.Should().Be(600000m);
        settings.Description.Should().Be("بروزرسانی تعرفه");
        settings.UpdatedAt.Should().NotBeNull();
    }
}
