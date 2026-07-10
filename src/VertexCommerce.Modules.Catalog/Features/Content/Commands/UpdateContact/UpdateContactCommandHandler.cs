using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Commands.UpdateContact;

internal sealed class UpdateContactCommandHandler(IContentRepository contentRepository)
    : ICommandHandler<UpdateContactCommand>
{
    public async Task<Result> Handle(UpdateContactCommand command, CancellationToken ct)
    {
        var doc = new ContactDocument
        {
            Title = command.Title,
            Subtitle = command.Subtitle,
            Description = command.Description,
            Email = command.Email,
            Phone = command.Phone,
            Address = command.Address,
            WorkingHours = command.WorkingHours,
            MapEmbedUrl = command.MapEmbedUrl,
            SocialLinks = (command.SocialLinks ?? new()).Select(s => new SocialLinkItem
            {
                Platform = s.Platform,
                Label = s.Label,
                Url = s.Url,
                Icon = s.Icon,
            }).ToList(),
        };

        await contentRepository.UpsertContactAsync(doc, ct);
        return Result.Success();
    }
}
