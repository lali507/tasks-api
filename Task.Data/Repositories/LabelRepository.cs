using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Results;
using TaskManager.Data.Extensions;

namespace TaskManager.Data.Repositories;

public class LabelRepository(TaskDbContext db) : ILabelRepository
{
    // FN-MB-05: ordered by Name, no paging
    public Task<List<Label>> GetAllAsync(CancellationToken ct) =>
        db.Labels.AsNoTracking().OrderBy(l => l.Name).ThenBy(l => l.Id).ToListAsync(ct);

    // Tracked, to attach to a task; duplicate ids are ignored (FN-AD-09-R2)
    public Task<List<Label>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken ct)
    {
        var idList = ids.Distinct().ToList();
        return db.Labels.Where(l => idList.Contains(l.Id)).ToListAsync(ct);
    }

    // Name is citext, so the comparison is case-insensitive
    public Task<bool> NameExistsAsync(string name, int? excludeId, CancellationToken ct) =>
        db.Labels.AsNoTracking().AnyAsync(l => l.Name == name && (excludeId == null || l.Id != excludeId), ct);

    public Task<Label?> FindAsync(int id, CancellationToken ct) =>
        db.Labels.FirstOrDefaultAsync(l => l.Id == id, ct);

    public Task<WriteResult> AddAsync(Label label, CancellationToken ct)
    {
        db.Labels.Add(label);
        return db.SaveAsync(ct);
    }

    public Task<WriteResult> UpdateAsync(Label label, CancellationToken ct)
    {
        if (db.Entry(label).State == EntityState.Detached)
            db.Labels.Update(label);
        return db.SaveAsync(ct);
    }

    // RE-03: the join table cascades, so the label is removed from its tasks
    public async Task DeleteAsync(Label label, CancellationToken ct)
    {
        db.Labels.Remove(label);
        await db.SaveChangesAsync(ct);
    }
}
