using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class AboutProcessSectionType : ObjectType<AboutProcessSection>
{
    protected override void Configure(IObjectTypeDescriptor<AboutProcessSection> descriptor)
    {
        descriptor.Name("AboutProcessSection");
        descriptor.Field(p => p.Badge).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Title).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Subtitle).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Steps).Type<NonNullType<ListType<NonNullType<AboutProcessStepItemType>>>>();
    }
}
