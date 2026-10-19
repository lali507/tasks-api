using TaskManager.Core.Entities;
using TaskManager.Core.Results;

namespace TaskManager.Core.Interfaces.Repositories;

// CO-04. Get* methods read with AsNoTracking (DA-03), except GetByIdsAsync; FindAsync tracks the entity.
public interface ILabelRepository
{
    // FN-MB-05: ordered by Name, no paging
    Task<List<Label>> GetAllAsync(CancellationToken ct);

    // Tracked, to attach to a task (FN-AD-09, FN-AD-10)
    Task<List<Label>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct);

    Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken ct);

    Task<Label?> FindAsync(int id, CancellationToken ct);
    Task<WriteResult> AddAsync(Label label, CancellationToken ct);
    Task<WriteResult> UpdateAsync(Label label, CancellationToken ct);
    Task DeleteAsync(Label label, CancellationToken ct);
}
