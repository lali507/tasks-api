namespace TaskManager.Core.DTOs.Responses;

// DT-08
public record ProjectResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }

    // EN-02-R4: DTO only, counted in the query
    public int TaskCount { get; init; }
}
