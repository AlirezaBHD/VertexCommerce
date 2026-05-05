using Microsoft.EntityFrameworkCore;

namespace VertexCommerce.Shared.Persistence.Outbox;

public interface IOutboxDbContext
{
    DbSet<OutboxMessage> OutboxMessages { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
