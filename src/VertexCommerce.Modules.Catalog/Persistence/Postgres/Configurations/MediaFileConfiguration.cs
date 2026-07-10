using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Catalog.Domain.Medias;

namespace VertexCommerce.Modules.Catalog.Persistence.Postgres.Configurations;

public sealed class MediaFileConfiguration : IEntityTypeConfiguration<MediaFile>
{
    public void Configure(EntityTypeBuilder<MediaFile> builder)
    {
        builder.ToTable("media_files", "catalog");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedNever();

        builder.Property(c => c.RelativePath)
            .HasColumnName("relative_path")
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(c => c.OriginalFileName)
            .HasColumnName("original_file_name")
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(c => c.ContentType)
            .HasColumnName("content_type")
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.SizeBytes)
            .HasColumnName("size_bytes");

        builder.Property(c => c.Status)
            .HasColumnName("status")
            .HasConversion<int>();

        builder.Property(c => c.ConfirmedAt)
            .HasColumnName("confirmed_at");

        builder.Ignore(c => c.DomainEvents);
    }
}
