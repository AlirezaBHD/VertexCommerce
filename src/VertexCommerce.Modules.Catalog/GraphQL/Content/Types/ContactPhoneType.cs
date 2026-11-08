using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class ContactPhoneType : ObjectType<ContactPhoneItem>
{
    protected override void Configure(IObjectTypeDescriptor<ContactPhoneItem> descriptor)
    {
        descriptor.Name("ContactPhone");
        descriptor.Field(p => p.Title).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Number).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Raw).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Badge).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Desc).Type<NonNullType<StringType>>();
    }
}
