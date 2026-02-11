using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Identity.Domain.Entities;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Identity.Persistence;

public sealed class IdentityDbContext : DbContext, IIdentityUnitOfWork
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("identity");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
