using Microsoft.EntityFrameworkCore;
using VertexCommerce.Modules.Orders.Domain.Entities;

namespace VertexCommerce.Modules.Orders.Persistence;

public sealed class OrdersDbContext : DbContext, IOrdersUnitOfWork
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<PaymentSettings> PaymentSettings => Set<PaymentSettings>();

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
