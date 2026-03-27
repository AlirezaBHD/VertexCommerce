using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.UpdateStock;

public sealed record UpdateStockCommand(Guid ProductId, int Quantity) : ICommand;

public sealed record AddStockCommand(Guid ProductId, int Quantity) : ICommand;

public sealed record RemoveStockCommand(Guid ProductId, int Quantity) : ICommand;
