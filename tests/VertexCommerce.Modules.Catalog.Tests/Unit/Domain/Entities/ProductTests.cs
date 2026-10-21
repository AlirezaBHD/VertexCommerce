using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.Products.Events;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;

namespace VertexCommerce.Modules.Catalog.Tests.Unit.Domain.Entities;

public class ProductTests
{
    private static Product CreateTestProduct(
        string name = "Test Product",
        string? description = "Test Description",
        Guid? categoryId = null,
        string slug = "test-product")
    {
        return Product.Create(
            ProductName.Create(name),
            ProductDescription.CreateOrNull(description),
            categoryId ?? Guid.NewGuid(),
            SeoMetadata.Create(Slug.Create(slug))
        );
    }

    [Fact]
    public void Create_WithValidData_ShouldCreateProductAndRaiseEvent()
    {
        // Arrange
        var categoryId = Guid.NewGuid();

        // Act
        var product = CreateTestProduct(categoryId: categoryId);

        // Assert
        product.Id.Should().NotBeEmpty();
        product.Name.Value.Should().Be("Test Product");
        product.Description.Should().NotBeNull();
        product.Description!.Value.Value.Should().Be("Test Description");
        product.CategoryId.Should().Be(categoryId);
        product.IsActive.Should().BeTrue();
        product.Variants.Should().BeEmpty();
        product.Media.Should().BeEmpty();

        var createdEvent = product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProductCreatedEvent>().Subject;
        createdEvent.ProductId.Should().Be(product.Id);
        createdEvent.Name.Should().Be("Test Product");
    }

    [Fact]
    public void Create_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var seo = SeoMetadata.Create(Slug.Create("test-slug"));

        // Act
        var act = () => Product.Create(
            default,
            ProductDescription.CreateOrNull("Description"),
            Guid.NewGuid(),
            seo
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void Update_ShouldUpdatePropertiesAndRaiseEvent()
    {
        // Arrange
        var product = CreateTestProduct();
        product.ClearDomainEvents();

        var newCategoryId = Guid.NewGuid();
        var newSeo = SeoMetadata.Create(Slug.Create("new-product-slug"));

        // Act
        product.Update(
            ProductName.Create("Updated Name"),
            ProductDescription.CreateOrNull("Updated Description"),
            newCategoryId,
            newSeo
        );

        // Assert
        product.Name.Value.Should().Be("Updated Name");
        product.Description!.Value.Value.Should().Be("Updated Description");
        product.CategoryId.Should().Be(newCategoryId);
        product.UpdatedAt.Should().NotBeNull();

        var updatedEvent = product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProductUpdatedEvent>().Subject;
        updatedEvent.ProductId.Should().Be(product.Id);
        updatedEvent.Name.Should().Be("Updated Name");
    }

    [Fact]
    public void Update_WithEmptyName_ShouldThrowException()
    {
        // Arrange
        var product = CreateTestProduct();

        // Act
        var act = () => product.Update(
            default,
            ProductDescription.CreateOrNull("Description"),
            Guid.NewGuid(),
            SeoMetadata.Create(Slug.Create("slug"))
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*name cannot be empty*");
    }

    [Fact]
    public void Delete_ShouldRaiseProductDeletedEvent()
    {
        // Arrange
        var product = CreateTestProduct();
        product.ClearDomainEvents();

        // Act
        product.Delete();

        // Assert
        var deletedEvent = product.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<ProductDeletedEvent>().Subject;
        deletedEvent.ProductId.Should().Be(product.Id);
    }

    [Fact]
    public void Activate_ShouldSetIsActiveTrue()
    {
        // Arrange
        var product = CreateTestProduct();
        product.Deactivate();

        // Act
        product.Activate();

        // Assert
        product.IsActive.Should().BeTrue();
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Deactivate_ShouldSetIsActiveFalse()
    {
        // Arrange
        var product = CreateTestProduct();

        // Act
        product.Deactivate();

        // Assert
        product.IsActive.Should().BeFalse();
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void ChangeCategory_ShouldUpdateCategoryId()
    {
        // Arrange
        var product = CreateTestProduct();
        var newCategoryId = Guid.NewGuid();

        // Act
        product.ChangeCategory(newCategoryId);

        // Assert
        product.CategoryId.Should().Be(newCategoryId);
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void AddVariant_WithMatchingProductId_ShouldAddVariant()
    {
        // Arrange
        var product = CreateTestProduct();
        var variant = ProductVariant.Create(
            product.Id,
            Sku.Create("VAR-001"),
            stockQuantity: 15,
            order: 1,
            Money.Create(49.99m, Currency.Create("USD")),
            []
        );

        // Act
        product.AddVariant(variant);

        // Assert
        product.Variants.Should().ContainSingle().Which.Should().Be(variant);
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void AddVariant_WithDifferentProductId_ShouldThrowArgumentException()
    {
        // Arrange
        var product = CreateTestProduct();
        var variant = ProductVariant.Create(
            Guid.NewGuid(), // Different ProductId
            Sku.Create("VAR-002"),
            stockQuantity: 5,
            order: 1,
            Money.Create(19.99m, Currency.Create("USD")),
            []
        );

        // Act
        var act = () => product.AddVariant(variant);

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*does not belong to this product*");
    }

    [Fact]
    public void RemoveVariant_WithExistingVariantId_ShouldRemoveVariant()
    {
        // Arrange
        var product = CreateTestProduct();
        var variant = ProductVariant.Create(
            product.Id,
            Sku.Create("VAR-001"),
            stockQuantity: 10,
            order: 1,
            Money.Create(29.99m, Currency.Create("USD")),
            []
        );
        product.AddVariant(variant);

        // Act
        product.RemoveVariant(variant.Id);

        // Assert
        product.Variants.Should().BeEmpty();
        product.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Media_Management_Add_Set_Remove_ShouldWorkCorrectly()
    {
        // Arrange
        var product = CreateTestProduct();
        var media1 = ProductMedia.Create(
            ImagePath.Create("/images/p1.jpg"),
            MediaType.Image,
            1,
            AltText.CreateOrNull("Photo 1")
        );
        var media2 = ProductMedia.Create(
            ImagePath.Create("/images/p2.jpg"),
            MediaType.Image,
            2,
            AltText.CreateOrNull("Photo 2")
        );

        // Act 1: Add media
        product.AddMedia(media1);
        product.Media.Should().ContainSingle().Which.Path.Value.Should().Be("/images/p1.jpg");

        // Act 2: Set media list
        product.SetMedia([media1, media2]);
        product.Media.Should().HaveCount(2);

        // Act 3: Remove media
        product.RemoveMedia("/images/p1.jpg");
        product.Media.Should().ContainSingle().Which.Path.Value.Should().Be("/images/p2.jpg");
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
}