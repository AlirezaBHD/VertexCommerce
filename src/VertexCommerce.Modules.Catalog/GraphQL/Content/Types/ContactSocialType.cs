using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class ContactSocialType : ObjectType<ContactSocialItem>
{
    protected override void Configure(IObjectTypeDescriptor<ContactSocialItem> descriptor)
    {
        descriptor.Name("ContactSocial");
        descriptor.Field(s => s.Platform).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Name).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Handle).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Url).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Badge).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Stats).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Description).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Icon).Type<NonNullType<StringType>>();
    }
}
