using FluentAssertions;
using VertexCommerce.Modules.Catalog.Domain.Entities;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Domain.Entities;

public class CategoryTests
{
    [Fact]
    public void Create_WithValidData_ShouldCreateCategory()
    {
        // Arrange
        var name = "Electronics";
        var description = "Electronic products";

        // Act
        var category = Category.Create(name, description);

        // Assert
        category.Id.Should().NotBeEmpty();
        category.Name.Should().Be(name);
        category.Description.Should().Be(description);
        category.ParentId.Should().BeNull();
        category.IsActive.Should().BeTrue();
        category.SortOrder.Should().Be(0);
    }

    [Fact]
    public void Create_WithParentId_ShouldSetParent()
    {
        // Arrange
        var parentId = Guid.NewGuid();

        // Act
        var category = Category.Create("Laptops", null, parentId);

        // Assert
        category.ParentId.Should().Be(parentId);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Create_WithEmptyName_ShouldThrowException(string? name)
    {
        // Act
        var act = () => Category.Create(name!);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void Update_ShouldUpdateCategory()
    {
        // Arrange
        var category = Category.Create("Old Name", "Old Description");

        // Act
        category.Update("New Name", "New Description", 5);

        // Assert
        category.Name.Should().Be("New Name");
        category.Description.Should().Be("New Description");
        category.SortOrder.Should().Be(5);
        category.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void SetParent_WithDifferentId_ShouldUpdateParent()
    {
        // Arrange
        var category = Category.Create("Test");
        var newParentId = Guid.NewGuid();

        // Act
        category.SetParent(newParentId);

        // Assert
        category.ParentId.Should().Be(newParentId);
    }

    [Fact]
    public void SetParent_WithSameId_ShouldThrowException()
    {
        // Arrange
        var category = Category.Create("Test");

        // Act
        var act = () => category.SetParent(category.Id);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*cannot be its own parent*");
    }

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        // Arrange
        var category = Category.Create("Test");
        category.Deactivate();

        // Act
        category.Activate();

        // Assert
        category.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        // Arrange
        var category = Category.Create("Test");

        // Act
        category.Deactivate();

        // Assert
        category.IsActive.Should().BeFalse();
    }
}