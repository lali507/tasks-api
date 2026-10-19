namespace TaskManager.Core.Results;

// What a repository write returns. Data translates the EF exceptions, because Service has no EF reference.
public enum WriteResult
{
    Success,
    Duplicate,           // unique index violation (DA-04, GN-12-R2)
    ConcurrencyConflict  // xmin changed since the read (FN-MB-06, FN-AD-10)
}
