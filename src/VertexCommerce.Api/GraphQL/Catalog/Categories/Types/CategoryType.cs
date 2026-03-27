using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;

namespace VertexCommerce.Api.GraphQL.Catalog.Categories;

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

        descriptor
            .Field(c => c.IsActive)
            .Type<NonNullType<BooleanType>>();

        descriptor
            .Field(c => c.SortOrder)
            .Type<NonNullType<IntType>>();

        descriptor
            .Field(c => c.Path)
            .Type<NonNullType<StringType>>()
            .Description("Full category path: Electronics > Phones > Samsung");

        descriptor
            .Field(c => c.Depth)
            .Type<NonNullType<IntType>>();

        descriptor
            .Field(c => c.ChildCount)
            .Type<NonNullType<IntType>>();

        descriptor
            .Field(c => c.ProductCount)
            .Type<NonNullType<IntType>>();
    }
}
