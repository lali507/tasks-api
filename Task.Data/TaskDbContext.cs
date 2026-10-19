using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;

namespace TaskManager.Data;

// ERD (document 2). Mapping (lengths, indexes, relationships, xmin) is added in task 8.
public class TaskDbContext(DbContextOptions<TaskDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TaskItem> TaskItems => Set<TaskItem>();
    public DbSet<Label> Labels => Set<Label>();
}
