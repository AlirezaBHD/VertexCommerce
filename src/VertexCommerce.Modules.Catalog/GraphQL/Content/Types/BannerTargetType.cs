using HotChocolate.Authorization;
using VertexCommerce.Modules.Catalog.Domain.Banners;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Modules.Catalog.Services;
using VertexCommerce.Shared.Contracts.Identity;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

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
