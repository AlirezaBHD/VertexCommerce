using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class AboutProcessStepItemType : ObjectType<AboutProcessStepItem>
{
    protected override void Configure(IObjectTypeDescriptor<AboutProcessStepItem> descriptor)
    {
        descriptor.Name("AboutProcessStepItem");
        descriptor.Field(s => s.Title).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Description).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Icon).Type<NonNullType<StringType>>();
    }
}
