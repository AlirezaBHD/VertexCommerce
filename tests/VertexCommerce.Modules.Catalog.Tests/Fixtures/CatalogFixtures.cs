using Bogus;
using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.ValueObjects;
using VertexCommerce.Modules.Catalog.Features.Categories.Commands.CreateCategory;
using VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;

namespace VertexCommerce.Modules.Catalog.Tests.Fixtures;

internal static class CatalogFixtures
{
    private static readonly Faker Faker = new();

    internal static class Products
    {
        public static Product Create(
            string? name = null,
            string? description = null,
            Guid? categoryId = null,
            string? slug = null)
        {
            var seo = SeoMetadata.Create(
                Slug.Create(slug ?? Faker.Lorem.Slug()),
                MetaTitle.CreateOrNull(Faker.Lorem.Sentence(3)),
                MetaDescription.CreateOrNull(Faker.Lorem.Sentence(6))
            );

            return Product.Create(
                ProductName.Create(name ?? Faker.Commerce.ProductName()),
                ProductDescription.CreateOrNull(description ?? Faker.Commerce.ProductDescription()),
                categoryId ?? Guid.NewGuid(),
                seo
            );
        }

        public static Product CreateWithVariant(
            string? name = null,
            decimal price = 99.99m,
            string currency = "USD",
            int stockQuantity = 10,
            Guid? categoryId = null)
        {
            var product = Create(name: name, categoryId: categoryId);
            var variant = ProductVariant.Create(
                product.Id,
                Sku.Generate(),
                stockQuantity,
                0,
                Money.Create(price, Currency.Create(currency)),
                []
            );
            product.AddVariant(variant);
            return product;
        }

        public static CreateProductCommand CreateCommand(
            string? name = null,
            string? description = null,
            Guid? categoryId = null,
            string? slug = null,
            decimal price = 99.99m,
            string currency = "USD",
            int stockQuantity = 10)
        {
            return new CreateProductCommand(
                name ?? Faker.Commerce.ProductName(),
                description ?? Faker.Commerce.ProductDescription(),
                categoryId ?? Guid.NewGuid(),
                new SeoMetadataRequest(
                    slug ?? Faker.Lorem.Slug(),
                    "Meta Title",
                    "Meta Description",
                    "keywords"
                ),
                [
                    new CreateVariantRequest(
                        price,
                        currency,
                        stockQuantity,
                        0,
                        []
                    )
                ],
                null
            );
        }
    }

    internal static class Categories
    {
        public static Category Create(
            string? name = null,
            string? description = null,
            Guid? parentId = null,
            int sortOrder = 0,
            bool isActive = true,
            string? coverImagePath = null)
        {
            return Category.Create(
                CategoryName.Create(name ?? Faker.Commerce.Categories(1)[0]),
                CategoryDescription.Create(description ?? Faker.Lorem.Sentence()),
                SeoMetadata.Create(Slug.Create(Faker.Lorem.Slug())),
                null,
                ImagePath.Create(coverImagePath ?? "/images/default-cover.jpg"),
                null,
                parentId,
                isActive,
                showOnHome: false,
                includeInMenu: true,
                sortOrder: sortOrder
            );
        }

        public static CreateCategoryCommand CreateCommand(
            string? name = null,
            string? description = null,
            Guid? parentId = null,
            int sortOrder = 0,
            string? slug = null,
            string? coverImagePath = null)
        {
            return new CreateCategoryCommand(
                name ?? Faker.Commerce.Categories(1)[0],
                description ?? Faker.Lorem.Sentence(),
                new SeoMetadataRequest(slug ?? Faker.Lorem.Slug(), "Meta Title", "Meta Description", null),
                null,
                coverImagePath ?? "/images/default-cover.jpg",
                null,
                parentId,
                IsActive: true,
                ShowOnHome: false,
                IncludeInMenu: true,
                SortOrder: sortOrder
            );
        }
    }
}
