using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Catalog.Domain.Entities;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Catalog.Persistence;

public sealed class CatalogDbContext : DbContext, IUnitOfWork
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<ProductAttribute> ProductAttributes => Set<ProductAttribute>();

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("catalog");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
