using VertexCommerce.Modules.Catalog.Features.Categories.Commands.CreateCategory;
using VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Features.CreateCategory;

public class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _validator = new();

    private static CreateCategoryCommand CreateValidCommand(
        string name = "Electronics",
        string description = "Valid Description",
        int sortOrder = 0)
    {
        return new CreateCategoryCommand(
            Name: name,
            Description: description,
            Seo: new SeoMetadataRequest("electronics", "Title", "Desc", null),
            IconPath: "/images/icon.png",
            CoverImagePath: "/images/cover.jpg",
            ImageAltText: "Alt Text",
            ParentId: null,
            IsActive: true,
            ShowOnHome: false,
            IncludeInMenu: true,
            SortOrder: sortOrder
        );
    }

    [Fact]
    public void Validate_WithValidCommand_ShouldBeValid()
    {
        // Arrange
        var command = CreateValidCommand();

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
        var command = CreateValidCommand(name: name!);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("required"));
    }

    [Fact]
    public void Validate_WithNameExceeding100Chars_ShouldBeInvalid()
    {
        // Arrange
        var command = CreateValidCommand(name: new string('A', 101));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Name" && e.ErrorMessage.Contains("100 characters"));
    }

    [Fact]
    public void Validate_WithDescriptionExceeding500Chars_ShouldBeInvalid()
    {
        // Arrange
        var command = CreateValidCommand(description: new string('A', 501));

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Description" && e.ErrorMessage.Contains("500 characters"));
    }

    [Fact]
    public void Validate_WithNegativeSortOrder_ShouldBeInvalid()
    {
        // Arrange
        var command = CreateValidCommand(sortOrder: -1);

        // Act
        var result = _validator.Validate(command);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "SortOrder" && e.ErrorMessage.Contains("negative"));
    }
}
