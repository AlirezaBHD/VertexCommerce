using MongoDB.Driver;
using VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories.Documents;

namespace VertexCommerce.Modules.Catalog.Persistence.Mongo.Categories;

internal static class CategoryQueryFilterBuilder
{
    public static FilterDefinition<CategoryReadModel> BuildListFilter(bool? isActive)
    {
        var filter = Builders<CategoryReadModel>.Filter.Empty;

        if (isActive.HasValue)
        {
            filter &= Builders<CategoryReadModel>.Filter
                .Eq(c => c.IsActive, isActive.Value);
        }

        return filter;
    }

    public static FilterDefinition<CategoryReadModel> BuildRootFilter()
    {
        return Builders<CategoryReadModel>.Filter.Eq(c => c.ParentId, null);
    }

    public static FilterDefinition<CategoryReadModel> BuildChildrenFilter(Guid parentId)
    {
        return Builders<CategoryReadModel>.Filter.Eq(c => c.ParentId, parentId);
    }

    public static SortDefinition<CategoryReadModel> BuildDefaultSort()
    {
        return Builders<CategoryReadModel>.Sort
            .Ascending(c => c.SortOrder)
            .Ascending(c => c.Name);
    }
}