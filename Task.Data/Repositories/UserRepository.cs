using Microsoft.EntityFrameworkCore;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Results;
using TaskManager.Data.Extensions;

namespace TaskManager.Data.Repositories;

public class UserRepository(TaskDbContext db) : IUserRepository
{
    public Task<bool> ExistsAsync(int id, CancellationToken ct) =>
        db.Users.AsNoTracking().AnyAsync(u => u.Id == id, ct);

    // Email is citext, so the comparison is case-insensitive
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct) =>
        db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Email == email, ct);

    public Task<bool> EmailExistsAsync(string email, int? excludeId, CancellationToken ct) =>
        db.Users.AsNoTracking().AnyAsync(u => u.Email == email && (excludeId == null || u.Id != excludeId), ct);

    // FN-AD-15: ordered by DisplayName; paging and count in the database (DA-03)
    public async Task<(List<User> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        var totalCount = await db.Users.CountAsync(ct);
        var items = await db.Users.AsNoTracking()
            .OrderBy(u => u.DisplayName).ThenBy(u => u.Id)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .ToListAsync(ct);
        return (items, totalCount);
    }

    public Task<User?> FindAsync(int id, CancellationToken ct) =>
        db.Users.FirstOrDefaultAsync(u => u.Id == id, ct);

    public Task<WriteResult> AddAsync(User user, CancellationToken ct)
    {
        db.Users.Add(user);
        return db.SaveAsync(ct);
    }

    public Task<WriteResult> UpdateAsync(User user, CancellationToken ct)
    {
        if (db.Entry(user).State == EntityState.Detached)
            db.Users.Update(user);
        return db.SaveAsync(ct);
    }

    // RE-02-R2: PostgreSQL sets TakenByUserId to null on the user's tasks
    public async Task DeleteAsync(User user, CancellationToken ct)
    {
        db.Users.Remove(user);
        await db.SaveChangesAsync(ct);
    }
}
