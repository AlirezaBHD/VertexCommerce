using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

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
