using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Identity.Domain.Entities;
using VertexCommerce.Shared.Persistence.Outbox;

namespace VertexCommerce.Modules.Identity.Persistence;

public sealed class IdentityDbContext : DbContext, IIdentityUnitOfWork, IOutboxDbContext
{

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

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
