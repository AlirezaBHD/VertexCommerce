using HotChocolate.Authorization;
using VertexCommerce.Modules.Catalog.Domain.Banners;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Modules.Catalog.Services;
using VertexCommerce.Shared.Contracts.Identity;

namespace VertexCommerce.Modules.Catalog.GraphQL.Content.Types;

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
