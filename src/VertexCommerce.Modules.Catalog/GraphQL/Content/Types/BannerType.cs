using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class BannerType : ObjectType<BannerDocument>
{
    protected override void Configure(IObjectTypeDescriptor<BannerDocument> descriptor)
    {
        descriptor.Name("Banner");

        descriptor.Field(b => b.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(h => h.Title).Type<NonNullType<StringType>>();
        descriptor.Field(h => h.RedirectPath).Type<NonNullType<StringType>>();
        descriptor.Field(b => b.ImagePath).Type<NonNullType<StringType>>();
        descriptor.Field(b => b.SortOrder).Type<NonNullType<StringType>>();
        descriptor.Field(b => b.IsActive).Type<NonNullType<BooleanType>>();
        descriptor.Field(b => b.CreatedAt).Type<NonNullType<DateTimeType>>();
        descriptor.Field(b => b.UpdatedAt).Type<NonNullType<DateTimeType>>();
    }
}
