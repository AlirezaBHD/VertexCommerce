using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Shared.Specifications;

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly CatalogDbContext _context;

    public ProductRepository(CatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Attributes)
            .ToListAsync(ct);
    }

    public Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default)
    {
        throw new NotImplementedException();
        // return await _context.Products
        //     .Include(p => p.Category)
        //     .Include(p => p.Attributes)
        //     .FirstOrDefaultAsync(p => p.Sku.Value == sku, ct);
    }

    public async Task<IReadOnlyList<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken ct = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Where(p => p.CategoryId == categoryId)
            .ToListAsync(ct);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Products.AnyAsync(p => p.Id == id, ct);
    }

    public Task<bool> SkuExistsAsync(string sku, CancellationToken ct = default)
    {
        throw new NotImplementedException();

        // return await _context.Products.AnyAsync(p => p.Sku.Value == sku, ct);
    }

    public async Task AddAsync(Product entity, CancellationToken ct = default)
    {
        await _context.Products.AddAsync(entity, ct);
    }

    public void Update(Product entity)
    {
        _context.Products.Update(entity);
    }

    public void Delete(Product entity)
    {
        _context.Products.Remove(entity);
    }

    public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(
        ISpecification<Product, TResult> spec,
        CancellationToken ct = default)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .AsQueryable();

        return await SpecificationEvaluator
            .ApplySpecification(query, spec)
            .ToListAsync(ct);
    }

    public async Task<int> CountAsync(
        ISpecification<Product> spec,
        CancellationToken ct = default)
    {
        var query = SpecificationEvaluator.ApplySpecification(
            _context.Products.AsQueryable(),
            spec);

        return await query.CountAsync(ct);
    }
    
    public async Task<bool> HasProductsInCategoryAsync(Guid categoryId, CancellationToken ct = default)
    {
        return await _context.Products.AnyAsync(p => p.CategoryId == categoryId, ct);
    }

    public async Task<Product?> GetByIdWithVariantsAsync(Guid id, CancellationToken ct)
    {
        return await _context.Products
            .Include(p => p.Variants)
            .ThenInclude(v => v.Media)
            .FirstOrDefaultAsync(p => p.Id == id, ct);
    }
}
