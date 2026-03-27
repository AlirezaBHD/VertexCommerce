using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Products.Commands.ToggleProductStatus;

public sealed record ActivateProductCommand(Guid Id) : ICommand;

public sealed record DeactivateProductCommand(Guid Id) : ICommand;
