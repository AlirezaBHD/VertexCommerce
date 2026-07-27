using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateHero;

public sealed record CreateOrUpdateHeroRequest(
    Guid? Id,
    string Title,
    string RedirectPath,
    Guid? ImageMediaFileId,
    Guid? MobileImageMediaFileId,
    Guid? VideoMediaFileId,
    string? ImagePath,
    string? MobileImagePath,
    string? VideoPath,
    bool IsActive = false);

public sealed record CreateOrUpdateHeroCommand(
    Guid? Id,
    string Title,
    string RedirectPath,
    Guid? ImageMediaFileId,
    Guid? MobileImageMediaFileId,
    Guid? VideoMediaFileId,
    string? ImagePath,
    string? MobileImagePath,
    string? VideoPath,
    bool IsActive = false) : ICommand<Guid>;
