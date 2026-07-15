using VertexCommerce.Modules.Catalog.Domain.Categories.Events;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Modules.Catalog.Domain.Products.ValueObjects;
using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Categories;

public sealed class Category : AggregateRoot<Guid>
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public SeoMetadata Seo { get; private set; } = null!;
    public string? IconPath { get; private set; }
    public string CoverImagePath { get; private set; } = string.Empty;
    public string? ImageAltText { get; private set; }
    public Guid? ParentId { get; private set; }
    public bool IsActive { get; private set; }
    public bool ShowOnHome { get; private set; }
    public bool IncludeInMenu { get; private set; }
    public int SortOrder { get; private set; }

    private readonly List<Category> _children = [];
    public IReadOnlyCollection<Category> Children => _children.AsReadOnly();

    private readonly List<Product> _products = [];
    public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

    private Category()
    {
    }

    public void Delete()
    {
        if (_children != null && _children.Any())
        {
            throw new InvalidOperationException("نمی‌توان دسته‌بندی که زیرمجموعه دارد را حذف کرد.");
        }
        
        SoftDelete();
        
        AddDomainEvent(new CategoryDeletedEvent(Id));
    }
    
    public static Category Create(
        string name,
        string description,
        SeoMetadata seoMetadata,
        string? iconPath,
        string coverImagePath,
        string? imageAltText,
        Guid? parentId,
        bool isActive,
        bool showOnHome,
        bool includeInMenu,
        int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name cannot be empty.", nameof(name));
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            Description = description.Trim(),
            ParentId = parentId,
            SortOrder = sortOrder,
            IsActive = isActive,
            Seo = seoMetadata,
            CoverImagePath = coverImagePath,
            IconPath = iconPath,
            ImageAltText = imageAltText,
            ShowOnHome = showOnHome,
            IncludeInMenu = includeInMenu
            
        };
        
        category.AddDomainEvent(new CategoryCreatedEvent(
            category.Id,
            category.Name,
            category.ParentId
        ));
        
        return category;
    }

    public void Update(
        string name,
        string description,
        SeoMetadata seoMetadata,
        string? iconPath,
        string coverImagePath,
        string? imageAltText,
        Guid? parentId,
        bool isActive,
        bool showOnHome,
        bool includeInMenu,
        int sortOrder)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name cannot be empty.", nameof(name));
        }

        Name = name.Trim();
        Description = description.Trim();
        ParentId = parentId;
        SortOrder = sortOrder;
        IsActive = isActive;
        Seo = seoMetadata;
        CoverImagePath = coverImagePath;
        IconPath = iconPath;
        ImageAltText = imageAltText;
        ShowOnHome = showOnHome;
        IncludeInMenu = includeInMenu;
        
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

    public void SetSortOrder(int sortOrder)
    {
        SortOrder = sortOrder;
        SetUpdatedAt();
        AddDomainEvent(new CategoryUpdatedEvent(Id, Name, ParentId, IsActive, sortOrder));
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