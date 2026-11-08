using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class AboutStorySectionType : ObjectType<AboutStorySection>
{
    protected override void Configure(IObjectTypeDescriptor<AboutStorySection> descriptor)
    {
        descriptor.Name("AboutStorySection");
        descriptor.Field(s => s.Badge).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Title).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Paragraphs).Type<NonNullType<ListType<NonNullType<StringType>>>>();
        descriptor.Field(s => s.ImagePath).Type<StringType>();
        descriptor.Field(s => s.ImageBadge).Type<StringType>();
        descriptor.Field(s => s.SupportText).Type<StringType>();
    }
}
