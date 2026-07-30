using VertexCommerce.Modules.Catalog.Domain.Banners;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Modules.Catalog.Services;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class HeroContentType : ObjectType<HeroContentDocument>
{
    protected override void Configure(IObjectTypeDescriptor<HeroContentDocument> descriptor)
    {
        descriptor.Name("HeroContent");

        descriptor.Field(h => h.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(h => h.Title).Type<NonNullType<StringType>>();
        descriptor.Field(h => h.Target).Type<BannerTargetType>();
        descriptor.Field(h => h.ImageMediaFileId).Type<UuidType>();
        descriptor.Field(h => h.ImagePath).Type<StringType>();
        descriptor.Field(h => h.MobileImageMediaFileId).Type<UuidType>();
        descriptor.Field(h => h.MobileImagePath).Type<StringType>();
        descriptor.Field(h => h.VideoMediaFileId).Type<UuidType>();
        descriptor.Field(h => h.VideoPath).Type<StringType>();
        descriptor.Field(h => h.IsActive).Type<NonNullType<BooleanType>>();
        descriptor.Field(h => h.UpdatedAt).Type<NonNullType<DateTimeType>>();

        descriptor.Field("href")
            .Type<StringType>()
            .Resolve(context =>
            {
                var hero = context.Parent<HeroContentDocument>();
                var resolver = context.Service<ITargetResolver>();
                return resolver.ResolveHref(hero.Target, out _);
            });

        descriptor.Field("isExternal")
            .Type<NonNullType<BooleanType>>()
            .Resolve(context =>
            {
                var hero = context.Parent<HeroContentDocument>();
                var resolver = context.Service<ITargetResolver>();
                resolver.ResolveHref(hero.Target, out var isExternal);
                return isExternal;
            });
    }
}
