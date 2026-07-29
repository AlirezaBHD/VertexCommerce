using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateBanner;

public sealed record CreateOrUpdateBannerRequest(
    Guid? Id,
    string Title,
    BannerTargetDto Target,
    Guid? MediaFileId,
    string? ImagePath,
    int SortOrder = 0,
    bool IsActive = true);

public sealed record CreateOrUpdateBannerCommand(
    Guid? Id,
    string Title,
    BannerTargetDto Target,
    Guid? MediaFileId,
    string? ImagePath,
    int SortOrder = 0,
    bool IsActive = true) : ICommand<Guid>;
