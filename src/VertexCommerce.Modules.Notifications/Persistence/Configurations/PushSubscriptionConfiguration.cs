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
            .HasMaxLength(2048)
            .IsRequired();

        builder.Property(s => s.P256dhKey)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(s => s.AuthKey)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(s => s.UserAgent)
            .HasMaxLength(512);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.Endpoint).IsUnique();
    }
}
