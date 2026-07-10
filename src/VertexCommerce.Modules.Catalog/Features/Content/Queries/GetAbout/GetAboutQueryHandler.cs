using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Queries.GetAbout;

internal sealed class GetAboutQueryHandler(IContentRepository contentRepository)
    : IQueryHandler<GetAboutQuery, AboutDocument>
{
    public async Task<Result<AboutDocument>> Handle(GetAboutQuery query, CancellationToken ct)
    {
        var doc = await contentRepository.GetAboutAsync(ct);
        return doc is null
            ? Result.Failure<AboutDocument>(Error.NotFound("About", "singleton"))
            : Result.Success(doc);
    }
}
