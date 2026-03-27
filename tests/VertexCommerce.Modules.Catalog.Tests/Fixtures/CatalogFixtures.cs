using Bogus;
using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
using VertexCommerce.Modules.Catalog.Features.Categories.Commands.CreateCategory;
using VertexCommerce.Modules.Catalog.Features.Products.Commands.CreateProduct;

namespace VertexCommerce.Modules.Catalog.Tests.Fixtures;

public static class CatalogFixtures
{
    private static readonly Faker Faker = new();

    public static class Products
    {
        public static Product Create(
            string? name = null,
            decimal? price = null,
            Guid? categoryId = null)
        {
            return Product.Create(
                name ?? Faker.Commerce.ProductName(),
                Faker.Commerce.ProductDescription(),
                Sku.Generate(),
                Money.Create(price ?? Faker.Random.Decimal(10, 1000), "USD"),
                Faker.Random.Int(0, 100),
                categoryId ?? Guid.NewGuid()
            );
        }

        public static CreateProductCommand CreateCommand(
            string? name = null,
            decimal? price = null,
            Guid? categoryId = null)
        {
            return new CreateProductCommand(
                name ?? Faker.Commerce.ProductName(),
                Faker.Commerce.ProductDescription(),
                null,
                price ?? Faker.Random.Decimal(10, 1000),
                "USD",
                Faker.Random.Int(0, 100),
                categoryId ?? Guid.NewGuid()
            );
        }
    }

    public static class Categories
    {
        public static Category Create(
            string? name = null,
            Guid? parentId = null)
        {
            return Category.Create(
                name ?? Faker.Commerce.Categories(1)[0],
                Faker.Lorem.Sentence(),
                parentId,
                Faker.Random.Int(0, 10)
            );
        }

        public static CreateCategoryCommand CreateCommand(
            string? name = null,
            Guid? parentId = null)
        {
            return new CreateCategoryCommand(
                name ?? Faker.Commerce.Categories(1)[0],
                Faker.Lorem.Sentence(),
                parentId,
                Faker.Random.Int(0, 10)
            );
        }
    }
}