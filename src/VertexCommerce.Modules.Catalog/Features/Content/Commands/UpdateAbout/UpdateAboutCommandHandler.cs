using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateAbout;

internal sealed class UpdateAboutCommandHandler(IContentRepository contentRepository)
    : ICommandHandler<UpdateAboutCommand>
{
    public async Task<Result> Handle(UpdateAboutCommand command, CancellationToken ct)
    {
        var doc = new AboutDocument
        {
            Title = command.Title,
            Subtitle = command.Subtitle,
            Description = command.Description,
            Mission = command.Mission,
            Vision = command.Vision,
            Values = (command.Values ?? new()).Select(v => new AboutValueItem
            {
                Icon = v.Icon,
                Title = v.Title,
                Description = v.Description,
            }).ToList(),
            Stats = (command.Stats ?? new()).Select(s => new AboutStatItem
            {
                Label = s.Label,
                Value = s.Value,
                Suffix = s.Suffix,
            }).ToList(),
            Team = (command.Team ?? new()).Select(t => new AboutTeamMember
            {
                Name = t.Name,
                Role = t.Role,
                Bio = t.Bio,
                ImagePath = t.ImagePath,
            }).ToList(),
        };

        await contentRepository.UpsertAboutAsync(doc, ct);
        return Result.Success();
    }
}
