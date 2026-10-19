using TaskManager.Core.DTOs.Queries;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Results;

namespace TaskManager.Core.Interfaces.Services;

// CO-04. currentUserId is checked on every write (GN-09-R1).
public interface IProjectService
{
    // FN-MB-01, FN-AD-01
    Task<ServiceResult<PagedResponse<ProjectResponse>>> GetPagedAsync(PagingQuery query, CancellationToken ct);

    // FN-MB-02, FN-AD-02
    Task<ServiceResult<ProjectDetailsResponse>> GetByIdAsync(int id, CancellationToken ct);

    // FN-AD-06
    Task<ServiceResult<ProjectResponse>> CreateAsync(int currentUserId, ProjectRequest request, CancellationToken ct);

    // FN-AD-07
    Task<ServiceResult<ProjectResponse>> UpdateAsync(int currentUserId, int id, ProjectRequest request, CancellationToken ct);

    // FN-AD-08
    Task<ServiceResult> DeleteAsync(int currentUserId, int id, CancellationToken ct);
}
