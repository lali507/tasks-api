using Riok.Mapperly.Abstractions;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Entities;

namespace TaskManager.Service.Mapping;

// SV-01, DT-08, DT-09
[Mapper]
[UseStaticMapper(typeof(TaskMapper))]
public static partial class ProjectMapper
{
    // EN-02-R3: taskCount comes from the COUNT in the repository query
    public static partial ProjectResponse ToResponse(this Project project, int taskCount);

    // The tasks are loaded here, so TaskCount is their number
    [MapProperty("Tasks.Count", nameof(ProjectDetailsResponse.TaskCount))]
    public static partial ProjectDetailsResponse ToDetailsResponse(this Project project);
}
