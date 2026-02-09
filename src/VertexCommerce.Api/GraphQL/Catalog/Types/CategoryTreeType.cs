namespace VertexCommerce.Api.GraphQL.Catalog.Types;

public sealed class CategoryTreeType
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
    public string? Description { get; init; }
    public bool IsActive { get; init; }
    public int SortOrder { get; init; }
    public List<CategoryTreeType> Children { get; init; } = [];
}
