using TaskManager.Core.Entities;
using TaskManager.Core.Results;

namespace TaskManager.Core.Interfaces.Repositories;

// CO-04. Get* methods read with AsNoTracking (DA-03); FindAsync tracks the entity for a change or delete.
public interface IUserRepository
{
    // GN-09-R1: does the user in the token still exist
    Task<bool> ExistsAsync(int id, CancellationToken ct);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct);
    Task<bool> EmailExistsAsync(string email, int? excludeId, CancellationToken ct);

    // FN-AD-15: ordered by DisplayName
    Task<(List<User> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct);

    Task<User?> FindAsync(int id, CancellationToken ct);
    Task<WriteResult> AddAsync(User user, CancellationToken ct);
    Task<WriteResult> UpdateAsync(User user, CancellationToken ct);
    Task DeleteAsync(User user, CancellationToken ct);
}
