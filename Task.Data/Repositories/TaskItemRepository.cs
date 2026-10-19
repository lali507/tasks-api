using Microsoft.EntityFrameworkCore;
using TaskManager.Core.DTOs.Queries;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Results;
using TaskManager.Data.Extensions;

namespace TaskManager.Data.Repositories;

public class TaskItemRepository(TaskDbContext db) : ITaskItemRepository
{
    // FN-MB-03: filters combined with AND; newest first; the page and its includes are one SQL query (DA-03)
    public async Task<(List<TaskItem> Items, int TotalCount)> GetPagedAsync(TaskQuery query, int currentUserId, CancellationToken ct)
    {
        var filtered = db.TaskItems.AsNoTracking();
        if (query.Status is { } status)
            filtered = filtered.Where(t => t.Status == status);
        if (query.ProjectId is { } projectId)
            filtered = filtered.Where(t => t.ProjectId == projectId);
        // FN-MB-03-R3; for an Admin this is always empty (FN-AD-03)
        if (query.Mine)
            filtered = filtered.Where(t => t.TakenByUserId == currentUserId);

        var totalCount = await filtered.CountAsync(ct);
        var items = await filtered
            .Include(t => t.Project)
            .Include(t => t.TakenByUser)
            .Include(t => t.Labels)
            .OrderByDescending(t => t.CreatedAt).ThenByDescending(t => t.Id)
            .Skip((query.Page - 1) * query.PageSize).Take(query.PageSize)
            .ToListAsync(ct);
        return (items, totalCount);
    }

    // FN-MB-04
    public Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct) =>
        db.TaskItems.AsNoTracking()
            .Include(t => t.Project)
            .Include(t => t.TakenByUser)
            .Include(t => t.Labels)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    // RE-02-R1
    public Task<bool> AnyTakenByUserAsync(int userId, CancellationToken ct) =>
        db.TaskItems.AsNoTracking().AnyAsync(t => t.TakenByUserId == userId && t.Status == Status.Taken, ct);

    // Tracked with labels only; the Service re-reads with GetByIdAsync for the response
    public Task<TaskItem?> FindAsync(int id, CancellationToken ct) =>
        db.TaskItems.Include(t => t.Labels).FirstOrDefaultAsync(t => t.Id == id, ct);

    public Task<WriteResult> AddAsync(TaskItem task, CancellationToken ct)
    {
        db.TaskItems.Add(task);
        return db.SaveAsync(ct);
    }

    // xmin is checked on save (EN-04-R3); a change since FindAsync returns ConcurrencyConflict
    public Task<WriteResult> UpdateAsync(TaskItem task, CancellationToken ct)
    {
        if (db.Entry(task).State == EntityState.Detached)
            db.TaskItems.Update(task);
        return db.SaveAsync(ct);
    }

    public Task<WriteResult> DeleteAsync(TaskItem task, CancellationToken ct)
    {
        db.TaskItems.Remove(task);
        return db.SaveAsync(ct);
    }
}
