namespace TaskManager.Core.DTOs.Queries;

// GN-02: defaults only; range checks are added in the API tasks
public record PagingQuery
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
