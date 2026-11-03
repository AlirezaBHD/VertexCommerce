using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Categories.Types;

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
