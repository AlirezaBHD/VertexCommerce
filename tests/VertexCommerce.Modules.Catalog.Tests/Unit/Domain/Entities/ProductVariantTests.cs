using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Domain.Entities;

public class ProductVariantTests
{
    private static ProductVariant CreateTestVariant(
        int stockQuantity = 20,
        decimal price = 100m,
        string sku = "TEST-SKU-001")
    {
        return ProductVariant.Create(
            productId: Guid.NewGuid(),
            sku: Sku.Create(sku),
            stockQuantity: stockQuantity,
            order: 0,
            price: Money.Create(price, Currency.Create("USD")),
            attributes: []
        );
    }

    [Fact]
    public void Create_WithValidData_ShouldInitializeProperties()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var sku = Sku.Create("TEST-VAR-1");
        var price = Money.Create(150m, Currency.Create("USD"));

        // Act
        var variant = ProductVariant.Create(
            productId,
            sku,
            stockQuantity: 25,
            order: 2,
            price,
            attributes: []
        );

        // Assert
        variant.Id.Should().NotBeEmpty();
        variant.ProductId.Should().Be(productId);
        variant.Sku.Should().Be(sku);
        variant.StockQuantity.Should().Be(25);
        variant.ReservedQuantity.Should().Be(0);
        variant.AvailableQuantity.Should().Be(25);
        variant.Price.Should().Be(price);
        variant.SortOrder.Should().Be(2);
        variant.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Create_WithNegativeStock_ShouldThrowException()
    {
        // Act
        var act = () => ProductVariant.Create(
            Guid.NewGuid(),
            Sku.Create("TEST-VAR-1"),
            stockQuantity: -5,
            order: 1,
            Money.Create(10m, Currency.Create("USD")),
            []
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void TryReserveStock_WhenAvailableStockSufficient_ShouldSucceed()
    {
        // Arrange
        var variant = CreateTestVariant(stockQuantity: 10);

        // Act
        var result = variant.TryReserveStock(4);

        // Assert
        result.Should().BeTrue();
        variant.ReservedQuantity.Should().Be(4);
        variant.AvailableQuantity.Should().Be(6);
        variant.StockQuantity.Should().Be(10);
        variant.UpdatedAt.Should().NotBeNull();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-2)]
    public void TryReserveStock_WithZeroOrNegativeQuantity_ShouldReturnFalse(int quantity)
    {
        // Arrange
        var variant = CreateTestVariant(stockQuantity: 10);

        // Act
        var result = variant.TryReserveStock(quantity);

        // Assert
        result.Should().BeFalse();
        variant.ReservedQuantity.Should().Be(0);
        variant.AvailableQuantity.Should().Be(10);
    }

    [Fact]
    public void TryReserveStock_WhenInsufficientStock_ShouldReturnFalse()
    {
        // Arrange
        var variant = CreateTestVariant(stockQuantity: 5);

        // Act
        var result = variant.TryReserveStock(6);

        // Assert
        result.Should().BeFalse();
        variant.ReservedQuantity.Should().Be(0);
        variant.AvailableQuantity.Should().Be(5);
    }

    [Fact]
    public void ReleaseReservedStock_WithValidQuantity_ShouldDecreaseReservedQuantity()
    {
        // Arrange
        var variant = CreateTestVariant(stockQuantity: 10);
        variant.TryReserveStock(6);

        // Act
        variant.ReleaseReservedStock(4);

        // Assert
        variant.ReservedQuantity.Should().Be(2);
        variant.AvailableQuantity.Should().Be(8);
        variant.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void ReleaseReservedStock_WhenReleasingMoreThanReserved_ShouldClampToZero()
    {
        // Arrange
        var variant = CreateTestVariant(stockQuantity: 10);
        variant.TryReserveStock(3);

        // Act
        variant.ReleaseReservedStock(5);

        // Assert
        variant.ReservedQuantity.Should().Be(0);
        variant.AvailableQuantity.Should().Be(10);
    }

    [Fact]
    public void TryCommitReservedStock_WhenReservedStockSufficient_ShouldDeductStockAndReserved()
    {
        // Arrange
        var variant = CreateTestVariant(stockQuantity: 10);
        variant.TryReserveStock(5);

        // Act
        var result = variant.TryCommitReservedStock(3);

        // Assert
        result.Should().BeTrue();
        variant.ReservedQuantity.Should().Be(2);
        variant.StockQuantity.Should().Be(7);
        variant.AvailableQuantity.Should().Be(5);
        variant.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void TryCommitReservedStock_WhenCommittingMoreThanReserved_ShouldReturnFalse()
    {
        // Arrange
        var variant = CreateTestVariant(stockQuantity: 10);
        variant.TryReserveStock(3);

        // Act
        var result = variant.TryCommitReservedStock(4);

        // Assert
        result.Should().BeFalse();
        variant.ReservedQuantity.Should().Be(3);
        variant.StockQuantity.Should().Be(10);
    }

    [Fact]
    public void TryDeductStock_WhenAvailableStockSufficient_ShouldDecreaseStockQuantity()
    {
        // Arrange
        var variant = CreateTestVariant(stockQuantity: 10);

        // Act
        var result = variant.TryDeductStock(4);

        // Assert
        result.Should().BeTrue();
        variant.StockQuantity.Should().Be(6);
        variant.AvailableQuantity.Should().Be(6);
        variant.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void TryDeductStock_WhenAvailableStockInsufficient_ShouldReturnFalse()
    {
        // Arrange
        var variant = CreateTestVariant(stockQuantity: 10);
        variant.TryReserveStock(8); // Available is 2

        // Act
        var result = variant.TryDeductStock(3);

        // Assert
        result.Should().BeFalse();
        variant.StockQuantity.Should().Be(10);
    }

    [Fact]
    public void UpdateStock_WithValidQuantity_ShouldUpdateStockQuantity()
    {
        // Arrange
        var variant = CreateTestVariant(stockQuantity: 10);

        // Act
        variant.UpdateStock(25);

        // Assert
        variant.StockQuantity.Should().Be(25);
        variant.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void UpdateStock_WithNegativeQuantity_ShouldThrowArgumentException()
    {
        // Arrange
        var variant = CreateTestVariant(stockQuantity: 10);

        // Act
        var act = () => variant.UpdateStock(-1);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void SetPrice_ShouldUpdatePrice()
    {
        // Arrange
        var variant = CreateTestVariant(price: 50m);
        var newPrice = Money.Create(75.50m, Currency.Create("USD"));

        // Act
        variant.SetPrice(newPrice);

        // Assert
        variant.Price.Should().Be(newPrice);
        variant.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void ActivateAndDeactivate_ShouldToggleIsActive()
    {
        // Arrange
        var variant = CreateTestVariant();

        // Act & Assert
        variant.Deactivate();
        variant.IsActive.Should().BeFalse();

        variant.Activate();
        variant.IsActive.Should().BeTrue();
    }
}
