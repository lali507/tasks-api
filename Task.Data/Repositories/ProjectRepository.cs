using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Results;
using TaskManager.Data.Extensions;

namespace TaskManager.Data.Repositories;

public class ProjectRepository(TaskDbContext db) : IProjectRepository
{
    public Task<bool> ExistsAsync(int id, CancellationToken ct) =>
        db.Projects.AsNoTracking().AnyAsync(p => p.Id == id, ct);

    // FN-MB-01: ordered by Name; TaskCount is a COUNT subquery, tasks are not loaded (EN-02-R3)
    public async Task<(List<(Project Project, int TaskCount)> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        var totalCount = await db.Projects.CountAsync(ct);
        var rows = await db.Projects.AsNoTracking()
            .OrderBy(p => p.Name).ThenBy(p => p.Id)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(p => new { Project = p, TaskCount = p.Tasks.Count })
            .ToListAsync(ct);
        return (rows.Select(r => (r.Project, r.TaskCount)).ToList(), totalCount);
    }

    // FN-MB-02: one query; tasks ordered by CreatedAt ascending, each with its labels and taker
    public Task<Project?> GetDetailsAsync(int id, CancellationToken ct) =>
        db.Projects.AsNoTracking()
            .Include(p => p.Tasks.OrderBy(t => t.CreatedAt)).ThenInclude(t => t.Labels)
            .Include(p => p.Tasks).ThenInclude(t => t.TakenByUser)
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    // Name is citext, so the comparison is case-insensitive
    public Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken ct) =>
        db.Projects.AsNoTracking().AnyAsync(p => p.Name == name && (excludeId == null || p.Id != excludeId), ct);

    // RE-01-R1
    public Task<bool> HasTasksAsync(int id, CancellationToken ct) =>
        db.TaskItems.AsNoTracking().AnyAsync(t => t.ProjectId == id, ct);

    public Task<Project?> FindAsync(int id, CancellationToken ct) =>
        db.Projects.FirstOrDefaultAsync(p => p.Id == id, ct);

    public Task<WriteResult> AddAsync(Project project, CancellationToken ct)
    {
        db.Projects.Add(project);
        return db.SaveAsync(ct);
    }

    public Task<WriteResult> UpdateAsync(Project project, CancellationToken ct)
    {
        if (db.Entry(project).State == EntityState.Detached)
            db.Projects.Update(project);
        return db.SaveAsync(ct);
    }

    public async Task DeleteAsync(Project project, CancellationToken ct)
    {
        db.Projects.Remove(project);
        await db.SaveChangesAsync(ct);
    }
}
