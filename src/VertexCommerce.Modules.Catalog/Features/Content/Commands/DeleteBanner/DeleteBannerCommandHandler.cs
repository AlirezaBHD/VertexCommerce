using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.DeleteBanner;

internal sealed class DeleteBannerCommandHandler(IContentRepository contentRepository)
    : ICommandHandler<DeleteBannerCommand>
{
    public async Task<Result> Handle(DeleteBannerCommand command, CancellationToken ct)
    {
        await contentRepository.DeleteBannerAsync(command.Id, ct);
        return Result.Success();
    }
}
