using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.GetProductById;

public sealed record GetProductByIdQuery(Guid Id) : IQuery<ProductResponse>;
