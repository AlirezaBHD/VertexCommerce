using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Queries.GetContact;

internal sealed class GetContactQueryHandler(IContentRepository contentRepository)
    : IQueryHandler<GetContactQuery, ContactDocument>
{
    public async Task<Result<ContactDocument>> Handle(GetContactQuery query, CancellationToken ct)
    {
        var doc = await contentRepository.GetContactAsync(ct);
        return doc is null
            ? Result.Failure<ContactDocument>(Error.NotFound("Contact", "singleton"))
            : Result.Success(doc);
    }
}
