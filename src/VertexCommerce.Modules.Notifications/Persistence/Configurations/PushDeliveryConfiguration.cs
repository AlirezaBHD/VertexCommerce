using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Notifications.Domain.Entities;

namespace VertexCommerce.Modules.Notifications.Persistence.Configurations;

internal sealed class PushDeliveryConfiguration : IEntityTypeConfiguration<PushDelivery>
{
    public void Configure(EntityTypeBuilder<PushDelivery> builder)
    {
        builder.ToTable("PushDeliveries");
        builder.HasKey(delivery => delivery.Id);
        builder.Property(delivery => delivery.Id).ValueGeneratedNever();
        builder.Property(delivery => delivery.NotificationId).IsRequired();
        builder.Property(delivery => delivery.SubscriptionId).IsRequired();
        builder.Property(delivery => delivery.Payload).IsRequired();
        builder.Property(delivery => delivery.ExpiresAt).IsRequired();
        builder.Property(delivery => delivery.NextAttemptAt).IsRequired();
        builder.Property(delivery => delivery.Outcome).HasConversion<string>().HasMaxLength(32);
        builder.HasOne<Notification>().WithMany().HasForeignKey(delivery => delivery.NotificationId);
        builder.HasIndex(delivery => new { delivery.NotificationId, delivery.SubscriptionId }).IsUnique();
        builder.HasIndex(delivery => new { delivery.NextAttemptAt, delivery.Id })
            .HasFilter("\"CompletedAt\" IS NULL");
    }
}
