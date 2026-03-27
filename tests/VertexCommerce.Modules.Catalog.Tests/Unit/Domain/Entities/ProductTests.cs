using FluentAssertions;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.Products.Events;
using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Domain.Entities;

public class ProductTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateProduct()
    {
        // Arrange
        var name = "Test Product";
        var sku = Sku.Create("TEST-001");
        var price = Money.Create(99.99m, "USD");
        var categoryId = Guid.NewGuid();

        // Act
        var product = Product.Create(name, "Description", sku, price, 10, categoryId);

        // Assert
        product.Id.Should().NotBeEmpty();
        product.Name.Should().Be(name);
        product.Sku.Should().Be(sku);
        product.Price.Should().Be(price);
        product.StockQuantity.Should().Be(10);
        product.CategoryId.Should().Be(categoryId);
        product.IsActive.Should().BeTrue();
        product.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Create_ShouldRaiseProductCreatedEvent()
    {
        // Arrange
        var sku = Sku.Create("TEST-001");
        var price = Money.Create(99.99m, "USD");

        // Act
        var product = Product.Create("Test", null, sku, price, 10, Guid.NewGuid());

        // Assert
        product.DomainEvents.Should().ContainSingle();
        product.DomainEvents.First().Should().BeOfType<ProductCreatedEvent>();

        var @event = (ProductCreatedEvent)product.DomainEvents.First();
        @event.ProductId.Should().Be(product.Id);
        @event.Name.Should().Be("Test");
        @event.Sku.Should().Be(sku.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ShouldThrowException(string? name)
    {
        // Arrange
        var sku = Sku.Create("TEST-001");
        var price = Money.Create(99.99m, "USD");

        // Act
        var act = () => Product.Create(name!, null, sku, price, 10, Guid.NewGuid());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void Create_WithNegativeStock_ShouldThrowException()
    {
        // Arrange
        var sku = Sku.Create("TEST-001");
        var price = Money.Create(99.99m, "USD");

        // Act
        var act = () => Product.Create("Test", null, sku, price, -1, Guid.NewGuid());

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*cannot be negative*");
    }

    [Fact]
    public void Update_ShouldUpdateProductAndRaiseEvent()
    {
        // Arrange
        var product = CreateTestProduct();
        var newPrice = Money.Create(149.99m, "USD");

        // Act
        product.Update("Updated Name", "New Description", newPrice);

        // Assert
        product.Name.Should().Be("Updated Name");
        product.Description.Should().Be("New Description");
        product.Price.Should().Be(newPrice);
        product.UpdatedAt.Should().NotBeNull();
        product.DomainEvents.Should().Contain(e => e is ProductUpdatedEvent);
    }

    [Fact]
    public void AddStock_WithPositiveQuantity_ShouldIncreaseStock()
    {
        // Arrange
        var product = CreateTestProduct();
        var initialStock = product.StockQuantity;

        // Act
        product.AddStock(5);

        // Assert
        product.StockQuantity.Should().Be(initialStock + 5);
    }

    [Fact]
    public void AddStock_WithZeroOrNegative_ShouldThrowException()
    {
        // Arrange
        var product = CreateTestProduct();

        // Act
        var act = () => product.AddStock(0);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*must be positive*");
    }

    [Fact]
    public void RemoveStock_WithValidQuantity_ShouldDecreaseStock()
    {
        // Arrange
        var product = CreateTestProduct();

        // Act
        product.RemoveStock(5);

        // Assert
        product.StockQuantity.Should().Be(5);
    }

    [Fact]
    public void RemoveStock_WhenInsufficientStock_ShouldThrowException()
    {
        // Arrange
        var product = CreateTestProduct();

        // Act
        var act = () => product.RemoveStock(100);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Insufficient stock*");
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalseAndRaiseEvent()
    {
        // Arrange
        var product = CreateTestProduct();

        // Act
        product.Deactivate();

        // Assert
        product.IsActive.Should().BeFalse();
        product.DomainEvents.Should().Contain(e => e is ProductDeletedEvent);
    }

    [Fact]
    public void AddAttribute_ShouldAddNewAttribute()
    {
        // Arrange
        var product = CreateTestProduct();

        // Act
        product.AddAttribute("Color", "Red", "string");

        // Assert
        product.Attributes.Should().ContainSingle();
        product.Attributes.First().Key.Should().Be("Color");
        product.Attributes.First().Value.Should().Be("Red");
    }

    [Fact]
    public void AddAttribute_WithExistingKey_ShouldReplaceAttribute()
    {
        // Arrange
        var product = CreateTestProduct();
        product.AddAttribute("Color", "Red");

        // Act
        product.AddAttribute("Color", "Blue");

        // Assert
        product.Attributes.Should().ContainSingle();
        product.Attributes.First().Value.Should().Be("Blue");
    }

    [Fact]
    public void RemoveAttribute_ShouldRemoveAttribute()
    {
        // Arrange
        var product = CreateTestProduct();
        product.AddAttribute("Color", "Red");

        // Act
        product.RemoveAttribute("Color");

        // Assert
        product.Attributes.Should().BeEmpty();
    }

    [Fact]
    public void ClearDomainEvents_ShouldRemoveAllEvents()
    {
        // Arrange
        var product = CreateTestProduct();
        product.DomainEvents.Should().NotBeEmpty();

        // Act
        product.ClearDomainEvents();

        // Assert
        product.DomainEvents.Should().BeEmpty();
    }

    private static Product CreateTestProduct()
    {
        return Product.Create(
            "Test Product",
            "Test Description",
            Sku.Create("TEST-001"),
            Money.Create(99.99m, "USD"),
            10,
            Guid.NewGuid()
        );
    }
}