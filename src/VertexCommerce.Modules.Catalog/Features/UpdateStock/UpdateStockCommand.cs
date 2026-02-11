using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.UpdateStock;

public sealed record UpdateStockCommand(Guid ProductId, int Quantity) : ICommand;

public sealed record AddStockCommand(Guid ProductId, int Quantity) : ICommand;

public sealed record RemoveStockCommand(Guid ProductId, int Quantity) : ICommand;
