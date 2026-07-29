using VertexCommerce.Modules.Catalog.Domain.Banners;

namespace VertexCommerce.Modules.Catalog.Features.Content.Queries;

public sealed record BannerResponseDto(
    Guid Id,
    string Title,
    BannerTarget Target,
    string? Href,
    bool IsExternal,
    Guid? MediaFileId,
    string? ImagePath,
    int SortOrder,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
