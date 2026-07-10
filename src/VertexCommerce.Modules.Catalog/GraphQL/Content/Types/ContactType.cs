using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class ContactType : ObjectType<ContactDocument>
{
    protected override void Configure(IObjectTypeDescriptor<ContactDocument> descriptor)
    {
        descriptor.Name("ContactContent");

        descriptor.Field(c => c.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(c => c.Title).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.Subtitle).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.Description).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.Email).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.Phone).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.Address).Type<NonNullType<StringType>>();
        descriptor.Field(c => c.WorkingHours).Type<StringType>();
        descriptor.Field(c => c.MapEmbedUrl).Type<StringType>();
        descriptor.Field(c => c.SocialLinks).Type<NonNullType<ListType<NonNullType<SocialLinkType>>>>();
        descriptor.Field(c => c.UpdatedAt).Type<NonNullType<DateTimeType>>();
    }
}

public sealed class SocialLinkType : ObjectType<SocialLinkItem>
{
    protected override void Configure(IObjectTypeDescriptor<SocialLinkItem> descriptor)
    {
        descriptor.Name("SocialLink");
        descriptor.Field(s => s.Platform).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Label).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Url).Type<NonNullType<StringType>>();
        descriptor.Field(s => s.Icon).Type<NonNullType<StringType>>();
    }
}
