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
            Header = command.Header ?? new(),
            Phones = command.Phones ?? new(),
            Emails = command.Emails ?? new(),
            WorkingHours = command.WorkingHours ?? new(),
            Location = command.Location ?? new(),
            Photos = command.Photos ?? new(),
            Socials = command.Socials ?? new(),
            Faqs = command.Faqs ?? new(),
            UpdatedAt = DateTime.UtcNow,
        };

        await contentRepository.UpsertContactAsync(doc, ct);
        return Result.Success();
    }
}
