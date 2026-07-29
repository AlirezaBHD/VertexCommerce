using VertexCommerce.Modules.Catalog.Domain.Banners;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateBanner;

public sealed record BannerTargetDto(
    TargetType Type,
    Guid? ProductId = null,
    string? ProductTitleSnapshot = null,
    string? ProductSlugSnapshot = null,
    string? ProductSkuSnapshot = null,
    Guid? CategoryId = null,
    string? CategoryTitleSnapshot = null,
    string? CategorySlugSnapshot = null,
    string? InternalPath = null,
    string? ExternalUrl = null);
