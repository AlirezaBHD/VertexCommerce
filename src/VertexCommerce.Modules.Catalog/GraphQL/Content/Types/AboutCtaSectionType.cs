using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class AboutCtaSectionType : ObjectType<AboutCtaSection>
{
    protected override void Configure(IObjectTypeDescriptor<AboutCtaSection> descriptor)
    {
        descriptor.Name("AboutCtaSection");
        descriptor.Field(c => c.Title).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.Subtitle).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.ButtonText).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.ButtonLink).Type<NonNullType<StringType>>();
    }
}
