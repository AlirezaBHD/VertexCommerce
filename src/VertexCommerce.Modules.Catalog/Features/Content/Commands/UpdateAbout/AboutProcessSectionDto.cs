namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;

public sealed record AboutProcessSectionDto(
    string? Badge,
    string? Title,
    string? Subtitle,
    List<AboutProcessStepItemDto>? Steps);
