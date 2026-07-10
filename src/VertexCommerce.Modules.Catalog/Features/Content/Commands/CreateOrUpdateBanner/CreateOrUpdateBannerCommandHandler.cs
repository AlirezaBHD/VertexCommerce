using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Modules.Catalog.Persistence.Postgres.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateBanner;

internal sealed class CreateOrUpdateBannerCommandHandler(
    IContentRepository contentRepository,
    IMediaFileRepository mediaFileRepository)
    : ICommandHandler<CreateOrUpdateBannerCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateOrUpdateBannerCommand command, CancellationToken ct)
    {
        string? imagePath = command.ImagePath;
        Guid? mediaFileId = command.MediaFileId;

        if (command.MediaFileId is not null)
        {
            var mediaFile = await mediaFileRepository.GetByIdAsync(command.MediaFileId.Value, ct);
            if (mediaFile is null)
                return Result.Failure<Guid>(Error.NotFound("MediaFile", command.MediaFileId.Value));

            imagePath = mediaFile.RelativePath;
            mediaFile.Confirm();
            await mediaFileRepository.SaveChangesAsync(ct);
        }

        var doc = new BannerDocument
        {
            Id = command.Id ?? Guid.NewGuid(),
            Title = command.Title,
            RedirectPath = command.RedirectPath,
            MediaFileId = mediaFileId,
            ImagePath = imagePath,
            SortOrder = command.SortOrder,
            IsActive = command.IsActive,
            CreatedAt = command.Id is null ? DateTime.UtcNow : default,
        };

        await contentRepository.UpsertBannerAsync(doc, ct);
        return Result.Success(doc.Id);
    }
}
