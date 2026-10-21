using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Shared.Exceptions;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Domain.ValueObjects;

public class SkuTests
{
    [Theory]
    [InlineData("PRD-001")]
    [InlineData("ABC123")]
    [InlineData("TEST-PRODUCT-123")]
    public void Create_WithValidSku_ShouldCreateSku(string value)
    {
        // Act
        var sku = Sku.Create(value);

        // Assert
        sku.Value.Should().Be(value.ToUpperInvariant());
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptySku_ShouldThrowException(string? value)
    {
        // Act
        var act = () => Sku.Create(value!);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*required*");
    }

    [Theory]
    [InlineData("AB")]
    [InlineData("A")]
    public void Create_WithTooShortSku_ShouldThrowException(string value)
    {
        // Act
        var act = () => Sku.Create(value);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*at least 3 characters*");
    }

    [Fact]
    public void Create_WithTooLongSku_ShouldThrowException()
    {
        // Arrange
        var longSku = new string('A', 51);

        // Act
        var act = () => Sku.Create(longSku);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*must not exceed 50 characters*");
    }

    [Theory]
    [InlineData("PRD@001")]
    [InlineData("PRD 001")]
    [InlineData("PRD_001")]
    public void Create_WithInvalidCharacters_ShouldThrowException(string value)
    {
        // Act
        var act = () => Sku.Create(value);

        // Assert
        act.Should().Throw<DomainValidationException>()
            .WithMessage("*only contain letters, numbers, and hyphens*");
    }

    [Fact]
    public void Create_ShouldNormalizeToUpperCaseAndTrim()
    {
        // Act
        var sku = Sku.Create("  prd-001  ");

        // Assert
        sku.Value.Should().Be("PRD-001");
    }

    [Fact]
    public void Generate_ShouldCreateValidSku()
    {
        // Act
        var sku = Sku.Generate("TEST");

        // Assert
        sku.Value.Should().StartWith("TEST-");
        sku.Value.Length.Should().BeGreaterThan(10);
    }

    [Fact]
    public void Generate_WithDefaultPrefix_ShouldUsePRD()
    {
        // Act
        var sku = Sku.Generate();

        // Assert
        sku.Value.Should().StartWith("PRD-");
    }

    [Fact]
    public void Equals_WithSameValue_ShouldBeEqual()
    {
        // Arrange
        var sku1 = Sku.Create("PRD-001");
        var sku2 = Sku.Create("prd-001");

        // Assert
        sku1.Should().Be(sku2);
        (sku1 == sku2).Should().BeTrue();
    }

    [Fact]
    public void Equals_WithDifferentValue_ShouldNotBeEqual()
    {
        // Arrange
        var sku1 = Sku.Create("PRD-001");
        var sku2 = Sku.Create("PRD-002");

        // Assert
        sku1.Should().NotBe(sku2);
        (sku1 != sku2).Should().BeTrue();
    }
}