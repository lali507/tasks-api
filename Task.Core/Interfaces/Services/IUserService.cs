using TaskManager.Core.DTOs.Queries;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Results;

namespace TaskManager.Core.Interfaces.Services;

// CO-04. currentUserId is checked on every write (GN-09-R1).
public interface IUserService
{
    // FN-AD-15
    Task<ServiceResult<PagedResponse<UserResponse>>> GetPagedAsync(PagingQuery query, CancellationToken ct);

    // FN-AD-16
    Task<ServiceResult<UserResponse>> CreateAsync(int currentUserId, CreateUserRequest request, CancellationToken ct);

    // FN-AD-17
    Task<ServiceResult<UserResponse>> UpdateAsync(int currentUserId, int id, UpdateUserRequest request, CancellationToken ct);

    // FN-AD-18
    Task<ServiceResult> DeleteAsync(int currentUserId, int id, CancellationToken ct);
}
