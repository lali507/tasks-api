using Microsoft.EntityFrameworkCore;
using Npgsql;
using TaskManager.Core.Results;

namespace TaskManager.Data.Extensions;

internal static class DbContextExtensions
{
    // DA-04, GN-12-R2: translates only the unique-index and concurrency exceptions; anything else propagates (MW-01)
    public static async Task<WriteResult> SaveAsync(this DbContext db, CancellationToken ct)
    {
        try
        {
            await db.SaveChangesAsync(ct);
            return WriteResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            db.ChangeTracker.Clear();
            return WriteResult.ConcurrencyConflict;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            db.ChangeTracker.Clear();
            return WriteResult.Duplicate;
        }
    }
}
