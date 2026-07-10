using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Queries.GetAllHero;

public sealed record GetAllHeroQuery : IQuery<IReadOnlyList<HeroContentDocument>>;
