using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;

public sealed record UpdateAboutCommand(
    AboutHeroSectionDto? Hero,
    AboutCommitmentsSectionDto? Commitments,
    AboutQualitySectionDto? Quality,
    AboutProcessSectionDto? Process,
    AboutStorySectionDto? Story,
    AboutCtaSectionDto? Cta) : ICommand;
