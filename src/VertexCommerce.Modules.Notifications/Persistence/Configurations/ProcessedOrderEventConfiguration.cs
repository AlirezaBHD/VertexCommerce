using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace VertexCommerce.Modules.Notifications.Persistence.Configurations;

internal sealed class ProcessedOrderEventConfiguration : IEntityTypeConfiguration<ProcessedOrderEvent>
{
    public void Configure(EntityTypeBuilder<ProcessedOrderEvent> builder)
    {
        builder.ToTable("ProcessedOrderEvents");
        builder.HasKey(orderEvent => orderEvent.EventId);
        builder.Property(orderEvent => orderEvent.EventId).ValueGeneratedNever();
    }
}
