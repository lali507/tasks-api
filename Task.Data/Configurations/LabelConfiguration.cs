using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Core.Entities;

namespace TaskManager.Data.Configurations;

// EN-03
public class LabelConfiguration : IEntityTypeConfiguration<Label>
{
    public void Configure(EntityTypeBuilder<Label> builder)
    {
        // DA-02: case-insensitive unique index (citext)
        builder.Property(l => l.Name).HasColumnType("citext").HasMaxLength(30);
        builder.HasIndex(l => l.Name).IsUnique();
    }
}
