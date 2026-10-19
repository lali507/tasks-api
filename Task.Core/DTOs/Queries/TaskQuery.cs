using TaskManager.Core.Enums;

namespace TaskManager.Core.DTOs.Queries;

// GN-02, FN-MB-03: filters for GET /api/tasks, combined with AND (FN-MB-03-R4)
public record TaskQuery : PagingQuery
{
    public Status? Status { get; init; }
    public int? ProjectId { get; init; }

    // FN-MB-03-R3: only tasks taken by the current user
    public bool Mine { get; init; }
}
