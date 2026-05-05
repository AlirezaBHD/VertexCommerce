using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Repositories;

public sealed class CategoryRepository(CatalogDbContext context) : ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Categories
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken ct = default)
    {
        return await context.Categories
            .Include(c => c.Children)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Category>> GetRootCategoriesAsync(CancellationToken ct = default)
    {
        return await context.Categories
            .Include(c => c.Children)
            .Where(c => c.ParentId == null)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<Category>> GetChildrenAsync(Guid parentId, CancellationToken ct = default)
    {
        return await context.Categories
            .Where(c => c.ParentId == parentId)
            .OrderBy(c => c.SortOrder)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Categories.AnyAsync(c => c.Id == id, ct);
    }

    public async Task<bool> NameExistsAsync(string name, Guid? excludeId = null, CancellationToken ct = default)
    {
        var query = context.Categories.Where(c => c.Name == name);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(ct);
    }

    public async Task AddAsync(Category entity, CancellationToken ct = default)
    {
        await context.Categories.AddAsync(entity, ct);
    }

    public void Update(Category entity)
    {
        context.Categories.Update(entity);
    }

    public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(
        ISpecification<Category, TResult> spec,
        CancellationToken ct = default)
    {
        return await SpecificationEvaluator
            .ApplySpecification(context.Categories.AsQueryable(), spec)
            .ToListAsync(ct);
    }

    public async Task<bool> HasChildrenAsync(Guid categoryId, CancellationToken ct = default)
    {
        return await context.Categories.AnyAsync(c => c.ParentId == categoryId, ct);
    }

    public async Task<bool> SlugExistsAsync(string slug, Guid? excludeId, CancellationToken ct)
    {
        var query = context.Categories.Where(c => c.Seo.Slug == slug);

        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }

        return await query.AnyAsync(ct);
    }
}
