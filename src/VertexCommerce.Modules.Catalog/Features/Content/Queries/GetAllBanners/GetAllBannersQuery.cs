using VertexCommerce.Modules.Catalog.Persistence.Mongo.Content.Documents;
using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Content.Queries.GetAllBanners;

public sealed record GetAllBannersQuery : IQuery<IReadOnlyList<BannerDocument>>;
