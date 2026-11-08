using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class ContactEmailType : ObjectType<ContactEmailItem>
{
    protected override void Configure(IObjectTypeDescriptor<ContactEmailItem> descriptor)
    {
        descriptor.Name("ContactEmail");
        descriptor.Field(e => e.Title).Type<NonNullType<StringType>>();
        descriptor.Field(e => e.Email).Type<NonNullType<StringType>>();
        descriptor.Field(e => e.Desc).Type<NonNullType<StringType>>();
    }
}
