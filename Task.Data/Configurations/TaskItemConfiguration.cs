using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TaskManager.Core.Entities;

namespace TaskManager.Data.Configurations;

// EN-04
public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItem>
{
    public void Configure(EntityTypeBuilder<TaskItem> builder)
    {
        builder.Property(t => t.Title).HasMaxLength(100);
        builder.Property(t => t.Description).HasMaxLength(1000);
        builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);

        // EN-04-R5: migration 1 has no CompletedAt. TEMPORARY: remove this line in task 12 (migration 2).
        builder.Ignore(t => t.CompletedAt);

        // EN-04-R3, GN-11-R1: maps to the PostgreSQL xmin system column, adds no column
        builder.Property(t => t.Xmin).IsRowVersion();

        // RE-01: required; a project with tasks cannot be deleted
        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Restrict);

        // RE-02: optional; deleting the user sets TakenByUserId to null (RE-02-R2)
        builder.HasOne(t => t.TakenByUser)
            .WithMany(u => u.TakenTasks)
            .HasForeignKey(t => t.TakenByUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // RE-03: many-to-many through TaskItemLabel; cascade removes only the links
        builder.HasMany(t => t.Labels)
            .WithMany(l => l.Tasks)
            .UsingEntity<Dictionary<string, object>>(
                "TaskItemLabel",
                right => right.HasOne<Label>().WithMany().HasForeignKey("LabelId").OnDelete(DeleteBehavior.Cascade),
                left => left.HasOne<TaskItem>().WithMany().HasForeignKey("TaskItemId").OnDelete(DeleteBehavior.Cascade),
                join => join.HasKey("TaskItemId", "LabelId"));
    }
}
