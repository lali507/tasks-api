using TaskManager.Core.DTOs.Queries;
using TaskManager.Core.Entities;
using TaskManager.Core.Results;

namespace TaskManager.Core.Interfaces.Repositories;

// CO-04. Get* methods read with AsNoTracking (DA-03); FindAsync tracks the entity for a change or delete.
public interface ITaskItemRepository
{
    // FN-MB-03: filtered, newest first, with project, taker and labels
    Task<(List<TaskItem> Items, int TotalCount)> GetPagedAsync(TaskQuery query, int currentUserId, CancellationToken ct);

    // FN-MB-04: with project, taker and labels
    Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct);

    // RE-02-R1: blocks deleting a user with a Taken task
    Task<bool> AnyTakenByUserAsync(int userId, CancellationToken ct);

    // Tracked, with labels, so the label list can be replaced (FN-AD-10)
    Task<TaskItem?> FindAsync(int id, CancellationToken ct);
    Task<WriteResult> AddAsync(TaskItem task, CancellationToken ct);
    Task<WriteResult> UpdateAsync(TaskItem task, CancellationToken ct);
    Task DeleteAsync(TaskItem task, CancellationToken ct);
}
