using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.SetActiveHero;

internal sealed class SetActiveHeroCommandHandler(IContentRepository contentRepository)
    : ICommandHandler<SetActiveHeroCommand>
{
    public async Task<Result> Handle(SetActiveHeroCommand command, CancellationToken ct)
    {
        await contentRepository.SetActiveHeroAsync(command.Id, ct);
        return Result.Success();
    }
}
