using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Data.Seed;

namespace TaskManager.Data;

// ERD (document 2). Mapping lives in Configurations/.
public class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<Label> Labels => Set<Label>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // DA-02: case-insensitive unique columns
        modelBuilder.HasPostgresExtension("citext");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TaskDbContext).Assembly);

        // GN-05
        modelBuilder.Seed();
    }
}
