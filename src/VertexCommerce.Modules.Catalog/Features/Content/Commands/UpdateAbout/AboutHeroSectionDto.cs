namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;

public sealed record AboutHeroSectionDto(
    string? Badge,
    string? Title,
    string? Subtitle,
    string? ButtonText,
    string? ImagePath,
    bool? ShowCat,
    string? CatImagePath);
