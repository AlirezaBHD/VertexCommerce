using VertexCommerce.Modules.Catalog.Domain.Banners;
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
        string? mobileImagePath = command.MobileImagePath;
        Guid? mobileImageMediaFileId = command.MobileImageMediaFileId;
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

        if (command.MobileImageMediaFileId is not null)
        {
            var mediaFile = await mediaFileRepository.GetByIdAsync(command.MobileImageMediaFileId.Value, ct);
            if (mediaFile is null)
                return Result.Failure<Guid>(Error.NotFound("MediaFile", command.MobileImageMediaFileId.Value));

            mobileImagePath = mediaFile.RelativePath;
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

        if (command.ImageMediaFileId is not null || command.MobileImageMediaFileId is not null || command.VideoMediaFileId is not null)
            await mediaFileRepository.SaveChangesAsync(ct);

        var target = new BannerTarget
        {
            Type = command.Target.Type,
            ProductId = command.Target.ProductId,
            ProductTitleSnapshot = command.Target.ProductTitleSnapshot,
            ProductSlugSnapshot = command.Target.ProductSlugSnapshot,
            ProductSkuSnapshot = command.Target.ProductSkuSnapshot,
            CategoryId = command.Target.CategoryId,
            CategoryTitleSnapshot = command.Target.CategoryTitleSnapshot,
            CategorySlugSnapshot = command.Target.CategorySlugSnapshot,
            InternalPath = command.Target.InternalPath,
            ExternalUrl = command.Target.ExternalUrl
        };

        var doc = new HeroContentDocument
        {
            Id = command.Id ?? Guid.NewGuid(),
            Title = command.Title,
            Target = target,
            ImageMediaFileId = imageMediaFileId,
            ImagePath = imagePath,
            MobileImageMediaFileId = mobileImageMediaFileId,
            MobileImagePath = mobileImagePath,
            VideoMediaFileId = videoMediaFileId,
            VideoPath = videoPath,
            IsActive = command.IsActive,
        };

        await contentRepository.UpsertHeroAsync(doc, ct);
        return Result.Success(doc.Id);
    }
}
