using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.DeleteHero;

internal sealed class DeleteHeroCommandHandler(IContentRepository contentRepository)
    : ICommandHandler<DeleteHeroCommand>
{
    public async Task<Result> Handle(DeleteHeroCommand command, CancellationToken ct)
    {
        await contentRepository.DeleteHeroAsync(command.Id, ct);
        return Result.Success();
    }
}
