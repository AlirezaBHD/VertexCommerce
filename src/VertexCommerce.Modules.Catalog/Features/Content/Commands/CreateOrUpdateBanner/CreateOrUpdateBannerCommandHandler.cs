using VertexCommerce.Modules.Catalog.Domain.Banners;
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

        var doc = new BannerDocument
        {
            Id = command.Id ?? Guid.NewGuid(),
            Title = command.Title,
            Target = target,
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
