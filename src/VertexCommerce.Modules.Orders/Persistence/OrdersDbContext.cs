using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Orders.Domain.Entities;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Orders.Persistence;

public sealed class OrdersDbContext : DbContext, IUnitOfWork
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("orders");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OrdersDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
