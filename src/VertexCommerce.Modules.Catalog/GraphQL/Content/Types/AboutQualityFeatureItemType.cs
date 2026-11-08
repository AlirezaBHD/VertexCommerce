using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class AboutQualityFeatureItemType : ObjectType<AboutQualityFeatureItem>
{
    protected override void Configure(IObjectTypeDescriptor<AboutQualityFeatureItem> descriptor)
    {
        descriptor.Name("AboutQualityFeatureItem");
        descriptor.Field(f => f.Title).Type<NonNullType<StringType>>();
        descriptor.Field(f => f.Description).Type<NonNullType<StringType>>();
    }
}
