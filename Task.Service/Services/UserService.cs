using Microsoft.AspNetCore.Identity;
using TaskManager.Core.DTOs.Queries;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Results;
using TaskManager.Service.Mapping;
using TaskManager.Service.Validation;

namespace TaskManager.Service.Services;

// Writes check in this order: user exists (401) -> resource exists (404) -> Admin (403) -> fields (400) -> business rules (409)
public class UserService(
    IUserRepository users,
    ITaskItemRepository tasks,
    IPasswordHasher<User> hasher,
    TimeProvider time) : IUserService
{
    // FN-AD-15: including Admins, ordered by DisplayName
    public async Task<ServiceResult<PagedResponse<UserResponse>>> GetPagedAsync(PagingQuery query, CancellationToken ct)
    {
        var (items, totalCount) = await users.GetPagedAsync(query.Page, query.PageSize, ct);
        return ServiceResult<PagedResponse<UserResponse>>.Success(new PagedResponse<UserResponse>
        {
            Items = items.Select(u => u.ToResponse()).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
        });
    }

    // FN-AD-16, UM-01, UM-02: always a Member, no token
    public async Task<ServiceResult<UserResponse>> CreateAsync(int currentUserId, CreateUserRequest request, CancellationToken ct)
    {
        // GN-09-R1
        if (!await users.ExistsAsync(currentUserId, ct))
            return ServiceResult<UserResponse>.Unauthorized();

        var errors = new FieldErrors();
        var email = errors.Email(nameof(request.Email), request.Email);
        var displayName = errors.Text(nameof(request.DisplayName), request.DisplayName, min: 2, max: 50);
        errors.Password(nameof(request.Password), request.Password);
        if (errors.HasErrors)
            return ServiceResult<UserResponse>.Validation(errors.ToDictionary());

        if (await users.EmailExistsAsync(email, null, ct))
            return ServiceResult<UserResponse>.Conflict(ErrorMessages.EmailTaken);

        var user = new User { Email = email, DisplayName = displayName, Role = Role.Member, CreatedAt = time.GetUtcNow().UtcDateTime };
        user.PasswordHash = hasher.HashPassword(user, request.Password);
        // GN-12: a parallel request with the same email fails on the unique index
        if (await users.AddAsync(user, ct) == WriteResult.Duplicate)
            return ServiceResult<UserResponse>.Conflict(ErrorMessages.EmailTaken);

        return ServiceResult<UserResponse>.Success(user.ToResponse());
    }

    // FN-AD-17, UM-02, UM-03: Role is never changed
    public async Task<ServiceResult<UserResponse>> UpdateAsync(int currentUserId, int id, UpdateUserRequest request, CancellationToken ct)
    {
        if (!await users.ExistsAsync(currentUserId, ct))
            return ServiceResult<UserResponse>.Unauthorized();

        // UM-06
        var user = await users.FindAsync(id, ct);
        if (user is null)
            return ServiceResult<UserResponse>.NotFound();

        // UM-03: including the Admin editing themself
        if (user.Role == Role.Admin)
            return ServiceResult<UserResponse>.Forbidden(ErrorMessages.AdminUserLocked);

        var errors = new FieldErrors();
        var email = errors.Email(nameof(request.Email), request.Email);
        var displayName = errors.Text(nameof(request.DisplayName), request.DisplayName, min: 2, max: 50);
        // FN-AD-17-R1: empty or null keeps the password
        var changePassword = !string.IsNullOrEmpty(request.NewPassword);
        if (changePassword)
            errors.Password(nameof(request.NewPassword), request.NewPassword);
        if (errors.HasErrors)
            return ServiceResult<UserResponse>.Validation(errors.ToDictionary());

        if (await users.EmailExistsAsync(email, id, ct))
            return ServiceResult<UserResponse>.Conflict(ErrorMessages.EmailTaken);

        user.Email = email;
        user.DisplayName = displayName;
        if (changePassword)
            user.PasswordHash = hasher.HashPassword(user, request.NewPassword!);
        if (await users.UpdateAsync(user, ct) == WriteResult.Duplicate)
            return ServiceResult<UserResponse>.Conflict(ErrorMessages.EmailTaken);

        return ServiceResult<UserResponse>.Success(user.ToResponse());
    }

    // FN-AD-18, UM-03 to UM-05
    public async Task<ServiceResult> DeleteAsync(int currentUserId, int id, CancellationToken ct)
    {
        if (!await users.ExistsAsync(currentUserId, ct))
            return ServiceResult.Unauthorized();

        var user = await users.FindAsync(id, ct);
        if (user is null)
            return ServiceResult.NotFound();

        if (user.Role == Role.Admin)
            return ServiceResult.Forbidden(ErrorMessages.AdminUserLocked);

        // UM-04, RE-02-R1
        if (await tasks.AnyTakenByUserAsync(id, ct))
            return ServiceResult.Conflict(ErrorMessages.UserHasActiveTask);

        // UM-05: the user's Done tasks stay Done with TakenByUserId = null
        await users.DeleteAsync(user, ct);
        return ServiceResult.Success();
    }
}
