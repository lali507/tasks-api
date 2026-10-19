using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Core.Entities;

namespace TaskManager.Data.Configurations;

// EN-01
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        // DA-02: case-insensitive unique index (citext)
        builder.Property(u => u.Email).HasColumnType("citext").HasMaxLength(256);
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.DisplayName).HasMaxLength(50);

        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
    }
}
