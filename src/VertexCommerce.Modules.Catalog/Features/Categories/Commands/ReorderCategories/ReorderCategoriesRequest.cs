namespace VertexCommerce.Modules.Catalog.Features.Categories.Commands.ReorderCategories;

public sealed record ReorderCategoriesRequest(IList<ReorderCategoryItem> Items);
