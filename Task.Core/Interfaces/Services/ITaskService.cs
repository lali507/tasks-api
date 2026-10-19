using TaskManager.Core.DTOs.Queries;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Results;

namespace TaskManager.Core.Interfaces.Services;

// CO-04. currentUserId is checked on every write (GN-09-R1).
public interface ITaskService
{
    // FN-MB-03, FN-AD-03; currentUserId is used for mine=true
    Task<ServiceResult<PagedResponse<TaskResponse>>> GetPagedAsync(int currentUserId, TaskQuery query, CancellationToken ct);

    // FN-MB-04, FN-AD-04
    Task<ServiceResult<TaskResponse>> GetByIdAsync(int id, CancellationToken ct);

    // FN-AD-09
    Task<ServiceResult<TaskResponse>> CreateAsync(int currentUserId, TaskRequest request, CancellationToken ct);

    // FN-AD-10
    Task<ServiceResult<TaskResponse>> UpdateAsync(int currentUserId, int id, TaskRequest request, CancellationToken ct);

    // FN-AD-11
    Task<ServiceResult> DeleteAsync(int currentUserId, int id, CancellationToken ct);

    // FN-MB-06
    Task<ServiceResult<TaskResponse>> TakeAsync(int currentUserId, int id, CancellationToken ct);

    // FN-MB-07
    Task<ServiceResult<TaskResponse>> CompleteAsync(int currentUserId, int id, CancellationToken ct);
}
