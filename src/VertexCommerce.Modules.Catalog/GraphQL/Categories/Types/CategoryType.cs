using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Categories.Types;

public sealed class CategoryType : ObjectType<CategoryReadModel>
{
    protected override void Configure(
        IObjectTypeDescriptor<CategoryReadModel> descriptor)
    {
        descriptor.Name("Category");

        descriptor
            .Field(c => c.Id)
            .Type<NonNullType<UuidType>>();

        descriptor
            .Field(c => c.Name)
            .Type<NonNullType<StringType>>();

        descriptor
            .Field(c => c.Description)
            .Type<StringType>();

        descriptor
            .Field(c => c.ParentId)
            .Type<UuidType>();

        // === Media ===
        descriptor
            .Field(c => c.IconPath)
            .Type<StringType>();

        descriptor
            .Field(c => c.CoverImagePath)
            .Type<NonNullType<StringType>>();

        descriptor
            .Field(c => c.ImageAltText)
            .Type<StringType>();

        // === Flags ===
        descriptor
            .Field(c => c.IsActive)
            .Type<NonNullType<BooleanType>>();

        descriptor
            .Field(c => c.ShowOnHome)
            .Type<NonNullType<BooleanType>>();

        descriptor
            .Field(c => c.IncludeInMenu)
            .Type<NonNullType<BooleanType>>();

        descriptor
            .Field(c => c.SortOrder)
            .Type<NonNullType<IntType>>();

        // === Breadcrumb ===
        descriptor
            .Field(c => c.Breadcrumb)
            .Type<NonNullType<ListType<NonNullType<CategoryBreadcrumbType>>>>()
            .Description("Ordered breadcrumb from root to this category");

        descriptor
            .Field(c => c.Depth)
            .Type<NonNullType<IntType>>();

        // === Children Summary ===
        descriptor
            .Field(c => c.ChildCount)
            .Type<NonNullType<IntType>>();

        descriptor
            .Field(c => c.ProductCount)
            .Type<NonNullType<IntType>>();

        // === Timestamps ===
        descriptor
            .Field(c => c.CreatedAt)
            .Type<NonNullType<DateTimeType>>();

        descriptor
            .Field(c => c.UpdatedAt)
            .Type<DateTimeType>();

        // === SEO Metadata ===
        descriptor
            .Field(c => c.Slug)
            .Type<NonNullType<StringType>>();

        descriptor
            .Field(c => c.MetaTitle)
            .Type<NonNullType<StringType>>();

        descriptor
            .Field(c => c.MetaDescription)
            .Type<NonNullType<StringType>>();

        descriptor
            .Field(c => c.Keywords)
            .Type<StringType>();
    }
}

public sealed class CategoryBreadcrumbType : ObjectType<CategoryBreadcrumb>
{
    protected override void Configure(IObjectTypeDescriptor<CategoryBreadcrumb> descriptor)
    {
        descriptor.Name("CategoryBreadcrumb");

        descriptor
            .Field(c => c.Id)
            .Type<NonNullType<UuidType>>();

        descriptor
            .Field(c => c.Name)
            .Type<NonNullType<StringType>>();

        descriptor
            .Field(c => c.Slug)
            .Type<NonNullType<StringType>>();
    }
}
