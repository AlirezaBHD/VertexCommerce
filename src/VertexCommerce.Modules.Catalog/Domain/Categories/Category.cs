using VertexCommerce.Modules.Catalog.Domain.Categories.Events;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Categories;

public sealed class Category : AggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid? ParentId { get; private set; }
    public bool IsActive { get; private set; }
    public int SortOrder { get; private set; }

    private readonly List<Category> _children = [];
    public IReadOnlyCollection<Category> Children => _children.AsReadOnly();

    private readonly List<Product> _products = [];
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category()
    {
    }

    public static Category Create(
        string name,
        string? description = null,
        Guid? parentId = null,
        int sortOrder = 0)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name cannot be empty.", nameof(name));
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description?.Trim(),
            ParentId = parentId,
            IsActive = true,
            SortOrder = sortOrder
        };
        
        category.AddDomainEvent(new CategoryCreatedEvent(
            category.Id,
            category.Name,
            category.ParentId
        ));
        
        return category;
    }

    public void Update(string name, string? description, Guid? parentId, int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name cannot be empty.", nameof(name));
        }

        Name = name;
        Description = description;
        ParentId = parentId;
        SortOrder = sortOrder;
        SetUpdatedAt();
        
        AddDomainEvent(new CategoryCreatedEvent(Id, Name, ParentId));
    }

    public void Activate()
    {
        IsActive = true;
        SetUpdatedAt();
    }

    public void Deactivate()
    {
        IsActive = false;
        SetUpdatedAt();
    }

    public void SetParent(Guid? parentId)
    {
        if (parentId == Id)
        {
            throw new InvalidOperationException("Category cannot be its own parent.");
        }

        ParentId = parentId;
        SetUpdatedAt();
    }
}