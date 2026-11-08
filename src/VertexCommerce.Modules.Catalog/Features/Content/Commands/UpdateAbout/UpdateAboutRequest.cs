namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;

public sealed record UpdateAboutRequest(
    AboutHeroSectionDto? Hero,
    AboutCommitmentsSectionDto? Commitments,
    AboutQualitySectionDto? Quality,
    AboutProcessSectionDto? Process,
    AboutStorySectionDto? Story,
    AboutCtaSectionDto? Cta);
