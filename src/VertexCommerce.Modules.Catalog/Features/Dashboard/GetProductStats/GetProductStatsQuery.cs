using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Dashboard.GetProductStats;

public sealed record GetProductStatsQuery : IQuery<ProductStatsResponse>;
