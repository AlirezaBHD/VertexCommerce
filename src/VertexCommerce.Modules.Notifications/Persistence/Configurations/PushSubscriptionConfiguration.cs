using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Notifications.Domain.Entities;

namespace VertexCommerce.Modules.Notifications.Persistence.Configurations;

public sealed class PushSubscriptionConfiguration : IEntityTypeConfiguration<PushSubscription>
{
    public void Configure(EntityTypeBuilder<PushSubscription> builder)
    {
        builder.ToTable("PushSubscriptions");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.UserId)
            .IsRequired();

        builder.Property(s => s.Endpoint)
            .HasMaxLength(PushSubscriptionLimits.Endpoint)
            .IsRequired();

        builder.Property(s => s.P256dhKey)
            .HasMaxLength(PushSubscriptionLimits.P256dhKey)
            .IsRequired();

        builder.Property(s => s.AuthKey)
            .HasMaxLength(PushSubscriptionLimits.AuthKey)
            .IsRequired();

        builder.Property(s => s.UserAgent)
            .HasMaxLength(PushSubscriptionLimits.UserAgent);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.Endpoint).IsUnique();
    }
}
