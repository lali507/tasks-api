using Riok.Mapperly.Abstractions;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Entities;

namespace TaskManager.Service.Mapping;

// SV-01, DT-13; needs Project, TakenByUser and Labels loaded
[Mapper]
[UseStaticMapper(typeof(LabelMapper))]
public static partial class TaskMapper
{
    [MapProperty("Project.Name", nameof(TaskResponse.ProjectName))]
    // FN-MB-02-R1: null when the task is Open or its taker was deleted
    [MapProperty("TakenByUser.DisplayName", nameof(TaskResponse.TakenByDisplayName))]
    public static partial TaskResponse ToResponse(this TaskItem task);
}
