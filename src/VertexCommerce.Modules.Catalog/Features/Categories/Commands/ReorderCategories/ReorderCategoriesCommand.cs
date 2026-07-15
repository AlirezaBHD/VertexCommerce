using VertexCommerce.Shared.CQRS;

namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.ReorderCategories;

public sealed record ReorderCategoriesCommand(IList<ReorderCategoryItem> Items) : ICommand;
