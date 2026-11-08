namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;

public sealed record AboutCommitmentsSectionDto(
    string? Badge,
    string? Title,
    string? Subtitle,
    List<AboutCommitmentItemDto>? Items);
