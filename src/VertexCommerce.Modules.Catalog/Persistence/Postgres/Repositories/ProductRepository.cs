using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Repositories;

public sealed class ProductRepository(CatalogDbContext context) : IProductRepository
{
    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await context.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<TResult?> GetByIdAsync<TResult>(Guid id, ISpecification<Product, TResult> spec,
        CancellationToken ct = default)
    {
        var query = context.Products
            .AsQueryable();

        return await SpecificationEvaluator
            .ApplySpecification(query, spec)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<IReadOnlyList<TResult>> GetAttributes<TResult>(ISpecification<CatalogAttribute, TResult> spec,
        CancellationToken ct = default)
    {
        var query = context.ProductAttributes
            .AsQueryable();

        return await SpecificationEvaluator
            .ApplySpecification(query, spec).ToListAsync(ct);
    }

    public async Task AddAsync(Product entity, CancellationToken ct = default)
    {
        await context.Products.AddAsync(entity, ct);
    }

    public async Task AddVariantAsync(ProductVariant variant, CancellationToken ct = default)
    {
       await context.Set<ProductVariant>().AddAsync(variant, ct);
    }

    public void Delete(Product entity)
    {
        context.Products.Remove(entity);
    }
    
    public async Task<bool> HasProductsInCategoryAsync(Guid categoryId, CancellationToken ct = default)
    {
        return await context.Products.AnyAsync(p => p.CategoryId == categoryId, ct);
    }

    public async Task<Product?> GetByIdWithVariantsAsync(Guid id, CancellationToken ct)
    {
        return await context.Products
            .Include(p => p.Media)
            .Include(p => p.Variants)
            .ThenInclude(v => v.Attributes)
            .Include(p => p.Variants)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<bool> SlugExistsAsync(string slug, CancellationToken ct)
    {
        return await context.Products.AnyAsync(p => p.Seo.Slug == slug, ct);
    }

    public void UpdateVariantAsync(ProductVariant variant)
    {
        context.Set<ProductVariant>().Update(variant);
    }

    public async Task<ProductVariant?> GetVariantByIdAsync(Guid variantId, CancellationToken ct = default)
    {
        return await context.Set<ProductVariant>()
            .FirstOrDefaultAsync(v => v.Id == variantId, ct);
    }
}
