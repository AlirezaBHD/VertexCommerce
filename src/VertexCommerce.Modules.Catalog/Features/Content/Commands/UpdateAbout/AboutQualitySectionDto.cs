namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;

public sealed record AboutQualitySectionDto(
    string? Badge,
    string? Title,
    List<string>? Paragraphs,
    List<AboutQualityFeatureItemDto>? Features,
    string? ImagePath,
    string? ImageBadgeTitle,
    string? ImageBadgeSubtitle);
