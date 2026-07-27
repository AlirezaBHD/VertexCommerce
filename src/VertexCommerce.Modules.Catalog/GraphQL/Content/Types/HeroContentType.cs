using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class HeroContentType : ObjectType<HeroContentDocument>
{
    protected override void Configure(IObjectTypeDescriptor<HeroContentDocument> descriptor)
    {
        descriptor.Name("HeroContent");

        descriptor.Field(h => h.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(h => h.Title).Type<NonNullType<StringType>>();
        descriptor.Field(h => h.RedirectPath).Type<NonNullType<StringType>>();
        descriptor.Field(h => h.ImageMediaFileId).Type<UuidType>();
        descriptor.Field(h => h.ImagePath).Type<StringType>();
        descriptor.Field(h => h.MobileImageMediaFileId).Type<UuidType>();
        descriptor.Field(h => h.MobileImagePath).Type<StringType>();
        descriptor.Field(h => h.VideoMediaFileId).Type<UuidType>();
        descriptor.Field(h => h.VideoPath).Type<StringType>();
        descriptor.Field(h => h.IsActive).Type<NonNullType<BooleanType>>();
        descriptor.Field(h => h.UpdatedAt).Type<NonNullType<DateTimeType>>();
    }
}
