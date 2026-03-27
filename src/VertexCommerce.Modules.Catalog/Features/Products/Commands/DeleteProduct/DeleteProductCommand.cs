using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.DeleteProduct;

public sealed record DeleteProductCommand(Guid Id) : ICommand;
