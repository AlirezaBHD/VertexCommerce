using VertexCommerce.Modules.Catalog.Domain.Banners;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Modules.Catalog.Services;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

public sealed class BannerType : ObjectType<BannerDocument>
{
    protected override void Configure(IObjectTypeDescriptor<BannerDocument> descriptor)
    {
        descriptor.Name("Banner");

        descriptor.Field(b => b.Id).Type<NonNullType<UuidType>>();
        descriptor.Field(b => b.Title).Type<NonNullType<StringType>>();
        descriptor.Field(b => b.Target).Type<BannerTargetType>();
        descriptor.Field(b => b.MediaFileId).Type<UuidType>();
        descriptor.Field(b => b.ImagePath).Type<StringType>();
        descriptor.Field(b => b.SortOrder).Type<NonNullType<IntType>>();
        descriptor.Field(b => b.IsActive).Type<NonNullType<BooleanType>>();
        descriptor.Field(b => b.CreatedAt).Type<NonNullType<DateTimeType>>();
        descriptor.Field(b => b.UpdatedAt).Type<NonNullType<DateTimeType>>();

        descriptor.Field("href")
            .Type<StringType>()
            .Resolve(context =>
            {
                var banner = context.Parent<BannerDocument>();
                var resolver = context.Service<ITargetResolver>();
                return resolver.ResolveHref(banner.Target, out _);
            });

        descriptor.Field("isExternal")
            .Type<NonNullType<BooleanType>>()
            .Resolve(context =>
            {
                var banner = context.Parent<BannerDocument>();
                var resolver = context.Service<ITargetResolver>();
                resolver.ResolveHref(banner.Target, out var isExternal);
                return isExternal;
            });
    }
}

public sealed class BannerTargetType : ObjectType<BannerTarget>
{
    protected override void Configure(IObjectTypeDescriptor<BannerTarget> descriptor)
    {
        descriptor.Name("BannerTarget");

        descriptor.Field(t => t.Type).Type<NonNullType<BannerTargetTypeEnum>>();
        descriptor.Field(t => t.ProductId).Type<UuidType>();
        descriptor.Field(t => t.ProductTitleSnapshot).Type<StringType>();
        descriptor.Field(t => t.ProductSlugSnapshot).Type<StringType>();
        descriptor.Field(t => t.ProductSkuSnapshot).Type<StringType>();
        descriptor.Field(t => t.CategoryId).Type<UuidType>();
        descriptor.Field(t => t.CategoryTitleSnapshot).Type<StringType>();
        descriptor.Field(t => t.CategorySlugSnapshot).Type<StringType>();
        descriptor.Field(t => t.InternalPath).Type<StringType>();
        descriptor.Field(t => t.ExternalUrl).Type<StringType>();
    }
}

public sealed class BannerTargetTypeEnum : EnumType<TargetType>
{
    protected override void Configure(IEnumTypeDescriptor<TargetType> descriptor)
    {
        descriptor.Name("BannerTargetType");
        descriptor.Value(TargetType.None).Name("None");
        descriptor.Value(TargetType.Product).Name("Product");
        descriptor.Value(TargetType.Category).Name("Category");
        descriptor.Value(TargetType.InternalPath).Name("InternalPath");
        descriptor.Value(TargetType.ExternalUrl).Name("ExternalUrl");
    }
}
