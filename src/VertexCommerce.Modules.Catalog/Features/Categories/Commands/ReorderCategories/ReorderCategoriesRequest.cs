namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.ReorderCategories;

public sealed record ReorderCategoryItem(Guid CategoryId, int SortOrder);

public sealed record ReorderCategoriesRequest(IList<ReorderCategoryItem> Items);
