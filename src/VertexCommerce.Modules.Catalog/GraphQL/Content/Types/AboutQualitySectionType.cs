using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class AboutQualitySectionType : ObjectType<AboutQualitySection>
{
    protected override void Configure(IObjectTypeDescriptor<AboutQualitySection> descriptor)
    {
        descriptor.Name("AboutQualitySection");
        descriptor.Field(q => q.Badge).Type<NonNullType<StringType>>();
        descriptor.Field(q => q.Title).Type<NonNullType<StringType>>();
        descriptor.Field(q => q.Paragraphs).Type<NonNullType<ListType<NonNullType<StringType>>>>();
        descriptor.Field(q => q.Features).Type<NonNullType<ListType<NonNullType<AboutQualityFeatureItemType>>>>();
        descriptor.Field(q => q.ImagePath).Type<StringType>();
        descriptor.Field(q => q.ImageBadgeTitle).Type<StringType>();
        descriptor.Field(q => q.ImageBadgeSubtitle).Type<StringType>();
    }
}
