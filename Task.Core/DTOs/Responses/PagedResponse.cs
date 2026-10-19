namespace TaskManager.Core.DTOs.Responses;

// DT-14; TotalCount is the count of all items, not of this page (DT-00-R3)
public record PagedResponse<T>
{
    public List<T> Items { get; init; } = [];
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
}
