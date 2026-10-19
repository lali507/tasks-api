using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Results;

namespace TaskManager.Core.Interfaces.Services;

// CO-04. Returns the user; the API creates the token (AP-04).
public interface IAuthService
{
    // FN-GU-01
    Task<ServiceResult<UserResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct);

    // FN-GU-02
    Task<ServiceResult<UserResponse>> LoginAsync(LoginRequest request, CancellationToken ct);
}
