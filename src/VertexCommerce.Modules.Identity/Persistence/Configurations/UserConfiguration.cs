using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VertexCommerce.Modules.Identity.Domain.Entities;
using VertexCommerce.Modules.Identity.Domain.ValueObjects;
using VertexCommerce.Shared.Persistence;

namespace VertexCommerce.Modules.Identity.Persistence.Configurations;
internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.ComplexProperty(u => u.PhoneNumber, phoneNumber => phoneNumber
            .Property(p => p.Value)
            .HasColumnName("PhoneNumber")
            .HasSchema(PhoneNumber.Schema));
        // builder.HasIndex("PhoneNumber.Value").IsUnique(); // Index created in migration
        builder.Property(u => u.PasswordHash)
            .HasMaxLength(512)
            .IsRequired();
        builder.ComplexProperty(u => u.FirstName, firstName => firstName
            .Property(p => p.Value)
            .HasColumnName("FirstName")
            .HasSchema(FirstName.Schema));
        builder.ComplexProperty(u => u.LastName, lastName => lastName
            .Property(p => p.Value)
            .HasColumnName("LastName")
            .HasSchema(LastName.Schema));
        builder.Property(u => u.Role)
            .HasConversion<string>()
            .HasMaxLength(20);
        builder.HasMany(u => u.RefreshTokens)
            .WithOne()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.Metadata
            .FindNavigation(nameof(User.RefreshTokens))!
            .SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
