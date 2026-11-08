using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class ContactType : ObjectType<ContactDocument>
{
    protected override void Configure(IObjectTypeDescriptor<ContactDocument> descriptor)
    {
        descriptor.Name("ContactContent");

        descriptor.Field(c => c.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(c => c.Header).Type<NonNullType<ContactHeaderType>>();
        descriptor.Field(c => c.Phones).Type<NonNullType<ListType<NonNullType<ContactPhoneType>>>>();
        descriptor.Field(c => c.Emails).Type<NonNullType<ListType<NonNullType<ContactEmailType>>>>();
        descriptor.Field(c => c.WorkingHours).Type<NonNullType<ContactWorkingHoursType>>();
        descriptor.Field(c => c.Location).Type<NonNullType<ContactLocationType>>();
        descriptor.Field(c => c.Photos).Type<NonNullType<ListType<NonNullType<ContactStorePhotoType>>>>();
        descriptor.Field(c => c.Socials).Type<NonNullType<ListType<NonNullType<ContactSocialType>>>>();
        descriptor.Field(c => c.Faqs).Type<NonNullType<ListType<NonNullType<ContactFaqType>>>>();
        descriptor.Field(c => c.UpdatedAt).Type<NonNullType<DateTimeType>>();
    }
}
