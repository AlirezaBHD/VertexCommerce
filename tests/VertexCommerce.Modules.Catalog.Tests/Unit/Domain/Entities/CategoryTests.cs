using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Categories.Events;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Domain.Entities;

public class CategoryTests
{
    private static Category CreateTestCategory(
        string name = "Electronics",
        string description = "Electronic products",
        Guid? parentId = null,
        int sortOrder = 0,
        bool isActive = true)
    {
        return Category.Create(
            CategoryName.Create(name),
            CategoryDescription.Create(description),
            SeoMetadata.Create(Slug.Create("electronics-slug")),
            iconPath: ImagePath.CreateOrNull("/images/icon.png"),
            coverImagePath: ImagePath.Create("/images/cover.jpg"),
            imageAltText: AltText.CreateOrNull("Cover Image"),
            parentId: parentId,
            isActive: isActive,
            showOnHome: true,
            includeInMenu: true,
            sortOrder: sortOrder
        );
    }

    [Fact]
    public void Create_WithValidData_ShouldCreateCategoryAndRaiseEvent()
    {
        // Arrange
        var parentId = Guid.NewGuid();

        // Act
        var category = CreateTestCategory(parentId: parentId, sortOrder: 3);

        // Assert
        category.Id.Should().NotBeEmpty();
        category.Name.Value.Should().Be("Electronics");
        category.Description.Value.Should().Be("Electronic products");
        category.ParentId.Should().Be(parentId);
        category.IsActive.Should().BeTrue();
        category.ShowOnHome.Should().BeTrue();
        category.IncludeInMenu.Should().BeTrue();
        category.SortOrder.Should().Be(3);
        category.CoverImagePath.Value.Should().Be("/images/cover.jpg");
        category.IconPath.Should().NotBeNull();
        category.IconPath!.Value.Value.Should().Be("/images/icon.png");

        category.DomainEvents.Should().ContainSingle();
        var createdEvent = category.DomainEvents.First().Should().BeOfType<CategoryCreatedEvent>().Subject;
        createdEvent.CategoryId.Should().Be(category.Id);
        createdEvent.Name.Should().Be("Electronics");
        createdEvent.ParentId.Should().Be(parentId);
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var description = CategoryDescription.Create("Valid Description");
        var seo = SeoMetadata.Create(Slug.Create("valid-slug"));
        var cover = ImagePath.Create("/images/cover.jpg");

        // Act
        var act = () => Category.Create(
            default,
            description,
            seo,
            null,
            cover,
            null,
            null,
            true,
            false,
            true,
            0
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void Update_WithValidData_ShouldUpdateProperties()
    {
        // Arrange
        var category = CreateTestCategory();
        category.ClearDomainEvents();

        var newSeo = SeoMetadata.Create(Slug.Create("updated-electronics"));

        // Act
        category.Update(
            CategoryName.Create("Updated Name"),
            CategoryDescription.Create("Updated Description"),
            newSeo,
            iconPath: null,
            coverImagePath: ImagePath.Create("/images/new-cover.jpg"),
            imageAltText: AltText.CreateOrNull("New Alt"),
            parentId: null,
            isActive: false,
            showOnHome: false,
            includeInMenu: false,
            sortOrder: 10
        );

        // Assert
        category.Name.Value.Should().Be("Updated Name");
        category.Description.Value.Should().Be("Updated Description");
        category.CoverImagePath.Value.Should().Be("/images/new-cover.jpg");
        category.IconPath.Should().BeNull();
        category.IsActive.Should().BeFalse();
        category.SortOrder.Should().Be(10);
        category.DomainEvents.Should().NotBeEmpty();
    }

    [Fact]
    public void Update_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var category = CreateTestCategory();

        // Act
        var act = () => category.Update(
            default,
            CategoryDescription.Create("Description"),
            SeoMetadata.Create(Slug.Create("slug")),
            null,
            ImagePath.Create("/images/cover.jpg"),
            null,
            null,
            true,
            false,
            true,
            0
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void SetParent_WithDifferentId_ShouldUpdateParent()
    {
        // Arrange
        var category = CreateTestCategory();
        var newParentId = Guid.NewGuid();

        // Act
        category.SetParent(newParentId);

        // Assert
        category.ParentId.Should().Be(newParentId);
        category.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void SetParent_WithSameId_ShouldThrowException()
    {
        // Arrange
        var category = CreateTestCategory();

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
        var category = CreateTestCategory(isActive: false);

        // Act
        category.Activate();

        // Assert
        category.IsActive.Should().BeTrue();
        category.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        // Arrange
        var category = CreateTestCategory(isActive: true);

        // Act
        category.Deactivate();

        // Assert
        category.IsActive.Should().BeFalse();
        category.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void SetSortOrder_ShouldUpdateSortOrderAndRaiseEvent()
    {
        // Arrange
        var category = CreateTestCategory(sortOrder: 0);
        category.ClearDomainEvents();

        // Act
        category.SetSortOrder(5);

        // Assert
        category.SortOrder.Should().Be(5);
        category.UpdatedAt.Should().NotBeNull();

        var updatedEvent = category.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<CategoryUpdatedEvent>().Subject;
        updatedEvent.CategoryId.Should().Be(category.Id);
        updatedEvent.SortOrder.Should().Be(5);
    }

    [Fact]
    public void Delete_WhenNoChildren_ShouldSoftDeleteAndRaiseEvent()
    {
        // Arrange
        var category = CreateTestCategory();
        category.ClearDomainEvents();

        // Act
        category.Delete();

        // Assert
        category.IsDeleted.Should().BeTrue();
        var deletedEvent = category.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<CategoryDeletedEvent>().Subject;
        deletedEvent.CategoryId.Should().Be(category.Id);
    }
}