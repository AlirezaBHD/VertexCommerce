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
            Header = command.Header,
            Phones = command.Phones ?? [],
            Emails = command.Emails ?? [],
            WorkingHours = command.WorkingHours ?? new ContactWorkingHoursSection(),
            Location = command.Location,
            Photos = command.Photos ?? [],
            Socials = command.Socials ?? [],
            Faqs = command.Faqs ?? [],
            UpdatedAt = DateTime.UtcNow,
        };

        await contentRepository.UpsertContactAsync(doc, ct);
        return Result.Success();
    }
}
