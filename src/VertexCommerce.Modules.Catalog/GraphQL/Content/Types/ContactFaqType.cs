using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class ContactFaqType : ObjectType<ContactFaqItem>
{
    protected override void Configure(IObjectTypeDescriptor<ContactFaqItem> descriptor)
    {
        descriptor.Name("ContactFaq");
        descriptor.Field(f => f.Question).Type<NonNullType<StringType>>();
        descriptor.Field(f => f.Answer).Type<NonNullType<StringType>>();
    }
}
