using TaskManager.Core.Entities;
using TaskManager.Core.Results;

namespace TaskManager.Core.Interfaces.Repositories;

// CO-04. Get* methods read with AsNoTracking (DA-03); FindAsync tracks the entity for a change or delete.
public interface IProjectRepository
{
    Task<bool> ExistsAsync(int id, CancellationToken ct);

    // FN-MB-01: ordered by Name; TaskCount is a COUNT in the same query (EN-02-R3)
    Task<(List<(Project Project, int TaskCount)> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct);

    // FN-MB-02: tasks with labels and taker, one query with Include/ThenInclude
    Task<Project?> GetDetailsAsync(int id, CancellationToken ct);

    Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken ct);
    Task<bool> HasTasksAsync(int id, CancellationToken ct);

    Task<Project?> FindAsync(int id, CancellationToken ct);
    Task<WriteResult> AddAsync(Project project, CancellationToken ct);
    Task<WriteResult> UpdateAsync(Project project, CancellationToken ct);
    Task DeleteAsync(Project project, CancellationToken ct);
}
