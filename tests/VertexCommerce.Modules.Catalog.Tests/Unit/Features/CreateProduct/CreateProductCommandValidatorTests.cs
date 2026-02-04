using FluentAssertions;
using VertexCommerce.Modules.Catalog.Features.CreateProduct;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Features.CreateProduct;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldBeValid()
    {
        // Arrange
        var command = new CreateProductCommand(
            "Valid Product",
            "Description",
            "SKU-001",
            99.99m,
            "USD",
            10,
            Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Validate_WithEmptyName_ShouldBeInvalid(string name)
    {
        // Arrange
        var command = new CreateProductCommand(
            name,
            null,
            null,
            99.99m,
            "USD",
            10,
            Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_WithNameTooLong_ShouldBeInvalid()
    {
        // Arrange
        var command = new CreateProductCommand(
            new string('A', 201),
            null,
            null,
            99.99m,
            "USD",
            10,
            Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Validate_WithNegativePrice_ShouldBeInvalid()
    {
        // Arrange
        var command = new CreateProductCommand(
            "Product",
            null,
            null,
            -10m,
            "USD",
            10,
            Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Price");
    }

    [Fact]
    public void Validate_WithNegativeStock_ShouldBeInvalid()
    {
        // Arrange
        var command = new CreateProductCommand(
            "Product",
            null,
            null,
            99.99m,
            "USD",
            -5,
            Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "StockQuantity");
    }

    [Theory]
    [InlineData("")]
    [InlineData("US")]
    [InlineData("USDD")]
    public void Validate_WithInvalidCurrency_ShouldBeInvalid(string currency)
    {
        // Arrange
        var command = new CreateProductCommand(
            "Product",
            null,
            null,
            99.99m,
            currency,
            10,
            Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Currency");
    }

    [Fact]
    public void Validate_WithEmptyCategoryId_ShouldBeInvalid()
    {
        // Arrange
        var command = new CreateProductCommand(
            "Product",
            null,
            null,
            99.99m,
            "USD",
            10,
            Guid.Empty
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CategoryId");
    }

    [Theory]
    [InlineData("AB")]
    [InlineData("SKU@123")]
    [InlineData("SKU 123")]
    public void Validate_WithInvalidSku_ShouldBeInvalid(string sku)
    {
        // Arrange
        var command = new CreateProductCommand(
            "Product",
            null,
            sku,
            99.99m,
            "USD",
            10,
            Guid.NewGuid()
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Sku");
    }
}