using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Catalog.Domain.Entities;
using VertexCommerce.Modules.Catalog.Domain.Repositories;

namespace VertexCommerce.Modules.Catalog.Persistence;

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

    public async Task<Product?> GetBySkuAsync(string sku, CancellationToken ct = default)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Attributes)
            .FirstOrDefaultAsync(p => p.Sku.Value == sku, ct);
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

    public async Task<bool> SkuExistsAsync(string sku, CancellationToken ct = default)
    {
        return await _context.Products.AnyAsync(p => p.Sku.Value == sku, ct);
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
}