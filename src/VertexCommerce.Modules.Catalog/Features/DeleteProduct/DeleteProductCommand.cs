using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : ICommand;
