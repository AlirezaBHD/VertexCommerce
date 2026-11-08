using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class AboutCommitmentsSectionType : ObjectType<AboutCommitmentsSection>
{
    protected override void Configure(IObjectTypeDescriptor<AboutCommitmentsSection> descriptor)
    {
        descriptor.Name("AboutCommitmentsSection");
        descriptor.Field(c => c.Badge).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.Title).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.Subtitle).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.Items).Type<NonNullType<ListType<NonNullType<AboutCommitmentItemType>>>>();
    }
}
