using Microsoft.AspNetCore.Identity;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Results;
using TaskManager.Core.Validation;
using TaskManager.Service.Mapping;
using TaskManager.Service.Validation;

namespace TaskManager.Service.Services;

// Returns the user; the API creates the token (AP-04)
public class AuthService(IUserRepository users, IPasswordHasher<User> hasher, TimeProvider time) : IAuthService
{
    // FN-GU-01, RL-01: always a Member; no check for a logged-in user (FN-GU-01-R1)
    public async Task<ServiceResult<UserResponse>> RegisterAsync(RegisterRequest request, CancellationToken ct)
    {
        var errors = new FieldErrors();
        var email = errors.Email(nameof(request.Email), request.Email);
        var displayName = errors.Text(nameof(request.DisplayName), request.DisplayName, min: 2, max: 50);
        errors.Password(nameof(request.Password), request.Password);
        if (errors.HasErrors)
            return ServiceResult<UserResponse>.Validation(errors.ToDictionary());

        if (await users.EmailExistsAsync(email, null, ct))
            return ServiceResult<UserResponse>.Conflict(ErrorMessages.EmailTaken);

        var user = new User { Email = email, DisplayName = displayName, Role = Role.Member, CreatedAt = time.GetUtcNow().UtcDateTime };
        // Only the hash is stored
        user.PasswordHash = hasher.HashPassword(user, request.Password);
        // GN-12: a parallel registration with the same email fails on the unique index
        if (await users.AddAsync(user, ct) == WriteResult.Duplicate)
            return ServiceResult<UserResponse>.Conflict(ErrorMessages.EmailTaken);

        return ServiceResult<UserResponse>.Success(user.ToResponse());
    }

    // FN-GU-02: one message for an unknown email and a wrong password; no lockout (FN-GU-02-R1)
    public async Task<ServiceResult<UserResponse>> LoginAsync(LoginRequest request, CancellationToken ct)
    {
        var errors = new FieldErrors();
        var email = errors.Email(nameof(request.Email), request.Email);
        // DT-02-R1: no length or complexity check
        if (string.IsNullOrEmpty(request.Password))
            errors.Add(nameof(request.Password), ValidationMessages.Required);
        if (errors.HasErrors)
            return ServiceResult<UserResponse>.Validation(errors.ToDictionary());

        var user = await users.GetByEmailAsync(email, ct);
        if (user is null || hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            return ServiceResult<UserResponse>.Unauthorized(ErrorMessages.InvalidCredentials);

        return ServiceResult<UserResponse>.Success(user.ToResponse());
    }
}
