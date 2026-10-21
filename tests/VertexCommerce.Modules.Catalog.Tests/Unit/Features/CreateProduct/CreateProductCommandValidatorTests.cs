using VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;
using VertexCommerce.Modules.Catalog.Tests.Fixtures;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Features.CreateProduct;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    [Fact]
    public void Validate_WithValidCommand_ShouldBeValid()
    {
        // Arrange
        var command = CatalogFixtures.Products.CreateCommand();

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Validate_WithEmptyName_ShouldBeInvalid(string? name)
    {
        // Arrange
        var command = new CreateProductCommand(
            Name: name!,
            Description: "Valid Description",
            CategoryId: Guid.NewGuid(),
            SeoMetadata: new SeoMetadataRequest("valid-slug", "Title", "Desc", null),
            Variants: null,
            Media: null
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("required"));
    }

    [Fact]
    public void Validate_WithNameTooLong_ShouldBeInvalid()
    {
        // Arrange
        var command = CatalogFixtures.Products.CreateCommand(name: new string('A', 201));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("200 characters"));
    }

    [Fact]
    public void Validate_WithDescriptionTooLong_ShouldBeInvalid()
    {
        // Arrange
        var command = CatalogFixtures.Products.CreateCommand(description: new string('A', 2001));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description" && e.ErrorMessage.Contains("2000 characters"));
    }

    [Fact]
    public void Validate_WithNullDescription_ShouldBeValid()
    {
        // Arrange
        var command = CatalogFixtures.Products.CreateCommand(description: null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithEmptyCategoryId_ShouldBeInvalid()
    {
        // Arrange
        var command = CatalogFixtures.Products.CreateCommand(categoryId: Guid.Empty);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CategoryId" && e.ErrorMessage.Contains("required"));
    }
}