using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

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
