using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateHero;

public sealed record CreateOrUpdateHeroRequest(
    Guid? Id,
    string Title,
    string RedirectPath,
    Guid? ImageMediaFileId,
    Guid? VideoMediaFileId,
    string? ImagePath,
    string? VideoPath,
    bool IsActive = false);

public sealed record CreateOrUpdateHeroCommand(
    Guid? Id,
    string Title,
    string RedirectPath,
    Guid? ImageMediaFileId,
    Guid? VideoMediaFileId,
    string? ImagePath,
    string? VideoPath,
    bool IsActive = false) : ICommand<Guid>;
