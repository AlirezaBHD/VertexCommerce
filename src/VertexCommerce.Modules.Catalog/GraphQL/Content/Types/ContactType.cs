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

public sealed class ContactWorkingHoursType : ObjectType<ContactWorkingHoursSection>
{
    protected override void Configure(IObjectTypeDescriptor<ContactWorkingHoursSection> descriptor)
    {
        descriptor.Name("ContactWorkingHours");
        descriptor.Field(w => w.IsOpenNow).Type<NonNullType<BooleanType>>();
        descriptor.Field(w => w.Items).Type<NonNullType<ListType<NonNullType<WorkingHourScheduleType>>>>();
    }
}

public sealed class WorkingHourScheduleType : ObjectType<WorkingHourScheduleItem>
{
    protected override void Configure(IObjectTypeDescriptor<WorkingHourScheduleItem> descriptor)
    {
        descriptor.Name("WorkingHourSchedule");
        descriptor.Field(i => i.Day).Type<NonNullType<StringType>>();
        descriptor.Field(i => i.Time).Type<NonNullType<StringType>>();
        descriptor.Field(i => i.Status).Type<NonNullType<StringType>>();
    }
}

public sealed class ContactLocationType : ObjectType<ContactLocationSection>
{
    protected override void Configure(IObjectTypeDescriptor<ContactLocationSection> descriptor)
    {
        descriptor.Name("ContactLocation");
        descriptor.Field(l => l.AddressText).Type<NonNullType<StringType>>();
        descriptor.Field(l => l.PostalCode).Type<NonNullType<StringType>>();
        descriptor.Field(l => l.MapImagePath).Type<StringType>();
        descriptor.Field(l => l.MapTitle).Type<NonNullType<StringType>>();
        descriptor.Field(l => l.MapSubtitle).Type<NonNullType<StringType>>();
        descriptor.Field(l => l.NeshanUrl).Type<StringType>();
        descriptor.Field(l => l.BaladUrl).Type<StringType>();
        descriptor.Field(l => l.GoogleMapsUrl).Type<StringType>();
    }
}

public sealed class ContactStorePhotoType : ObjectType<ContactStorePhotoItem>
{
    protected override void Configure(IObjectTypeDescriptor<ContactStorePhotoItem> descriptor)
    {
        descriptor.Name("ContactStorePhoto");
        descriptor.Field(p => p.Url).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.ImagePath).Type<StringType>();
        descriptor.Field(p => p.Title).Type<NonNullType<StringType>>();
        descriptor.Field(p => p.Tag).Type<NonNullType<StringType>>();
    }
}

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

public sealed class ContactFaqType : ObjectType<ContactFaqItem>
{
    protected override void Configure(IObjectTypeDescriptor<ContactFaqItem> descriptor)
    {
        descriptor.Name("ContactFaq");
        descriptor.Field(f => f.Question).Type<NonNullType<StringType>>();
        descriptor.Field(f => f.Answer).Type<NonNullType<StringType>>();
    }
}
