using TaskManager.Core.Enums;

namespace TaskManager.Core.DTOs.Responses;

// DT-13; Status is serialized as a string (DT-00-R5); never xmin (DT-13-R1)
public record TaskResponse
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public Status Status { get; init; }
    public int ProjectId { get; init; }
    public string ProjectName { get; init; } = string.Empty;
    public int? TakenByUserId { get; init; }
    public string? TakenByDisplayName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public List<LabelResponse> Labels { get; init; } = [];
}
