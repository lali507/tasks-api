namespace TaskManager.Core.DTOs.Responses;

// DT-09: all DT-08 fields plus the project's tasks
public record ProjectDetailsResponse : ProjectResponse
{
    public List<TaskResponse> Tasks { get; init; } = [];
}
