using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class ContactHeaderType : ObjectType<ContactHeaderSection>
{
    protected override void Configure(IObjectTypeDescriptor<ContactHeaderSection> descriptor)
    {
        descriptor.Name("ContactHeader");
        descriptor.Field(h => h.Badge).Type<NonNullType<StringType>>();
        descriptor.Field(h => h.Title).Type<NonNullType<StringType>>();
        descriptor.Field(h => h.Description).Type<NonNullType<StringType>>();
    }
}
