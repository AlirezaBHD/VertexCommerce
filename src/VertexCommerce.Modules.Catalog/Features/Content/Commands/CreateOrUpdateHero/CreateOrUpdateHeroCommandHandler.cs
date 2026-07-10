using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Modules.Catalog.Persistence.Postgres.Repositories;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.CreateOrUpdateHero;

internal sealed class CreateOrUpdateHeroCommandHandler(
    IContentRepository contentRepository,
    IMediaFileRepository mediaFileRepository)
    : ICommandHandler<CreateOrUpdateHeroCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateOrUpdateHeroCommand command, CancellationToken ct)
    {
        string? imagePath = command.ImagePath;
        Guid? imageMediaFileId = command.ImageMediaFileId;
        string? videoPath = command.VideoPath;
        Guid? videoMediaFileId = command.VideoMediaFileId;

        if (command.ImageMediaFileId is not null)
        {
            var mediaFile = await mediaFileRepository.GetByIdAsync(command.ImageMediaFileId.Value, ct);
            if (mediaFile is null)
                return Result.Failure<Guid>(Error.NotFound("MediaFile", command.ImageMediaFileId.Value));

            imagePath = mediaFile.RelativePath;
            mediaFile.Confirm();
        }

        if (command.VideoMediaFileId is not null)
        {
            var mediaFile = await mediaFileRepository.GetByIdAsync(command.VideoMediaFileId.Value, ct);
            if (mediaFile is null)
                return Result.Failure<Guid>(Error.NotFound("MediaFile", command.VideoMediaFileId.Value));

            videoPath = mediaFile.RelativePath;
            mediaFile.Confirm();
        }

        if (command.ImageMediaFileId is not null || command.VideoMediaFileId is not null)
            await mediaFileRepository.SaveChangesAsync(ct);

        var doc = new HeroContentDocument
        {
            Id = command.Id ?? Guid.NewGuid(),
            Title = command.Title,
            RedirectPath = command.RedirectPath,
            ImageMediaFileId = imageMediaFileId,
            ImagePath = imagePath,
            VideoMediaFileId = videoMediaFileId,
            VideoPath = videoPath,
            IsActive = command.IsActive,
        };

        await contentRepository.UpsertHeroAsync(doc, ct);
        return Result.Success(doc.Id);
    }
}
