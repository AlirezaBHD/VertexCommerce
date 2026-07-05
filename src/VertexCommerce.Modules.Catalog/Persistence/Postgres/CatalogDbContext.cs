using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Catalog.Domain.Categories;
using VertexCommerce.Modules.Catalog.Domain.Medias;
using VertexCommerce.Modules.Catalog.Domain.Products;
using VertexCommerce.Shared.Persistence.Outbox;

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres;

public sealed class CatalogDbContext : DbContext, ICatalogUnitOfWork, IOutboxDbContext
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<MediaFile> MediaFiles => Set<MediaFile>();
    public DbSet<CatalogAttribute> ProductAttributes => Set<CatalogAttribute>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

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
