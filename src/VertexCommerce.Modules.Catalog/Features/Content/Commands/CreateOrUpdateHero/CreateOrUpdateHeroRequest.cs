using VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateBanner;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateHero;

public sealed record CreateOrUpdateHeroRequest(
    Guid? Id,
    string Title,
    BannerTargetDto Target,
    Guid? ImageMediaFileId,
    Guid? MobileImageMediaFileId,
    Guid? VideoMediaFileId,
    string? ImagePath,
    string? MobileImagePath,
    string? VideoPath,
    bool IsActive = false);