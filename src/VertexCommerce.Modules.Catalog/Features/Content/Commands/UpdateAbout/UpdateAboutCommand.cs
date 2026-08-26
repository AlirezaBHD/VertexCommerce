using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;

public sealed record UpdateAboutRequest(
    AboutHeroSectionDto? Hero,
    AboutCommitmentsSectionDto? Commitments,
    AboutQualitySectionDto? Quality,
    AboutProcessSectionDto? Process,
    AboutStorySectionDto? Story,
    AboutCtaSectionDto? Cta);

public sealed record UpdateAboutCommand(
    AboutHeroSectionDto? Hero,
    AboutCommitmentsSectionDto? Commitments,
    AboutQualitySectionDto? Quality,
    AboutProcessSectionDto? Process,
    AboutStorySectionDto? Story,
    AboutCtaSectionDto? Cta) : ICommand;

public sealed record AboutHeroSectionDto(
    string? Badge,
    string? Title,
    string? Subtitle,
    string? ButtonText,
    string? ImagePath,
    bool? ShowCat,
    string? CatImagePath);

public sealed record AboutCommitmentsSectionDto(
    string? Badge,
    string? Title,
    string? Subtitle,
    List<AboutCommitmentItemDto>? Items);

public sealed record AboutCommitmentItemDto(
    string? Title,
    string? Description,
    string? Badge,
    string? Icon);

public sealed record AboutQualitySectionDto(
    string? Badge,
    string? Title,
    List<string>? Paragraphs,
    List<AboutQualityFeatureItemDto>? Features,
    string? ImagePath,
    string? ImageBadgeTitle,
    string? ImageBadgeSubtitle);

public sealed record AboutQualityFeatureItemDto(
    string? Title,
    string? Description);

public sealed record AboutProcessSectionDto(
    string? Badge,
    string? Title,
    string? Subtitle,
    List<AboutProcessStepItemDto>? Steps);

public sealed record AboutProcessStepItemDto(
    string? Title,
    string? Description,
    string? Icon);

public sealed record AboutStorySectionDto(
    string? Badge,
    string? Title,
    List<string>? Paragraphs,
    string? ImagePath,
    string? ImageBadge,
    string? SupportText);

public sealed record AboutCtaSectionDto(
    string? Title,
    string? Subtitle,
    string? ButtonText,
    string? ButtonLink);
