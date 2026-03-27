using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.DeleteCategory;

public sealed record DeleteCategoryCommand(Guid Id) : ICommand;
