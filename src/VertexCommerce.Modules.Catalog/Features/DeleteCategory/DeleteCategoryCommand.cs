using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid Id) : ICommand;
