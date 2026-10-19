namespace TaskManager.Core.DTOs.Responses;

// DT-11
public record LabelResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}
