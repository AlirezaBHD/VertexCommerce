using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Queries.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IQuery<ProductResponse>;
