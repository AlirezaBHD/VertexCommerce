using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class AboutCommitmentItemType : ObjectType<AboutCommitmentItem>
{
    protected override void Configure(IObjectTypeDescriptor<AboutCommitmentItem> descriptor)
    {
        descriptor.Name("AboutCommitmentItem");
        descriptor.Field(i => i.Title).Type<NonNullType<StringType>>();
        descriptor.Field(i => i.Description).Type<NonNullType<StringType>>();
        descriptor.Field(i => i.Badge).Type<NonNullType<StringType>>();
        descriptor.Field(i => i.Icon).Type<NonNullType<StringType>>();
    }
}
