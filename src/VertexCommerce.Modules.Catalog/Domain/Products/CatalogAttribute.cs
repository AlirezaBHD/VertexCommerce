using VertexCommerce.Shared.Domain;

namespace VertexCommerce.Modules.Catalog.Domain.Products;

public sealed class CatalogAttribute : Entity<Guid>
{
    public string Code { get; private set; } = string.Empty;
    public string DefaultName { get; private set; } = string.Empty;
    public string? Type { get; private set; }

    private readonly List<CatalogAttributeOption> _options = new();
    public IReadOnlyList<CatalogAttributeOption> Options => _options.AsReadOnly();

    private CatalogAttribute()
    {
    }

    public static CatalogAttribute Create(string code, string defaultName, string? type = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Catalog attribute cannot be empty.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(defaultName))
        {
            throw new ArgumentException("Catalog attribute default name cannot be empty.", nameof(defaultName));
        }

        return new CatalogAttribute
        {
            Id = Guid.NewGuid(),
            Code = code.Trim().ToLowerInvariant(),
            DefaultName = defaultName.Trim(),
            Type = type?.Trim()
        };
    }

    public void Update(string code, string defaultName, string? type = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Catalog attribute code cannot be empty.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(defaultName))
        {
            throw new ArgumentException("Catalog attribute default name cannot be empty.", nameof(defaultName));
        }

        DefaultName = code.Trim();
        Type = type?.Trim();
        SetUpdatedAt();
    }

    public void AddOption(string defaultName, string optionCode, string? mediaPath = null)
    {
        _options.Add(CatalogAttributeOption.Create(Id, defaultName, optionCode, mediaPath));
    }
}
