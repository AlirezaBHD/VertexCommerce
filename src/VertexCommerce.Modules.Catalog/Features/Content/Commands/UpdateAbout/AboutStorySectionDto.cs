namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;

public sealed record AboutStorySectionDto(
    string? Badge,
    string? Title,
    List<string>? Paragraphs,
    string? ImagePath,
    string? ImageBadge,
    string? SupportText);
