using Microsoft.AspNetCore.Identity;
using Moq;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Results;
using TaskManager.Service.Services;
using TaskManager.Tests.Infrastructure;

namespace TaskManager.Tests.Services;

public class UserServiceTests
{
    private const int AdminId = 1;
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<ITaskItemRepository> _tasks = new();
    private readonly PasswordHasher<User> _hasher = new();
    private readonly UserService _service;

    public UserServiceTests()
    {
        _users.Setup(u => u.ExistsAsync(AdminId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _users.Setup(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.Success);
        _users.Setup(u => u.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.Success);
        _users.Setup(u => u.FindAsync(AdminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = AdminId, Email = "admin@example.com", Role = Role.Admin });
        _service = new UserService(_users.Object, _tasks.Object, _hasher, new FixedTimeProvider());
    }

    private User GivenMember(int id, string password = "OldPass123")
    {
        var user = new User { Id = id, Email = $"member{id}@example.com", DisplayName = "Member", Role = Role.Member };
        user.PasswordHash = _hasher.HashPassword(user, password);
        _users.Setup(u => u.FindAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(user);
        return user;
    }

    // TC-U-04, UM-01
    [Fact]
    public async Task Create_ExistingEmail_ReturnsConflict()
    {
        _users.Setup(u => u.EmailExistsAsync("member02@example.com", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.CreateAsync(AdminId,
            new CreateUserRequest { Email = "Member02@example.com", DisplayName = "Dana", Password = "Password1" }, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.EmailTaken, result.Title);
    }

    // UM-02, RL-01: an Admin creates Members only
    [Fact]
    public async Task Create_ValidUser_IsAlwaysMember()
    {
        User? saved = null;
        _users.Setup(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => saved = user)
            .ReturnsAsync(WriteResult.Success);

        var result = await _service.CreateAsync(AdminId,
            new CreateUserRequest { Email = "new@example.com", DisplayName = "New", Password = "Password1" }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(Role.Member, saved!.Role);
    }

    // TC-U-06, FN-AD-17-R1: no NewPassword keeps the old one
    [Fact]
    public async Task Update_WithoutNewPassword_KeepsPassword()
    {
        var user = GivenMember(5);
        var oldHash = user.PasswordHash;

        var result = await _service.UpdateAsync(AdminId, 5,
            new UpdateUserRequest { Email = user.Email, DisplayName = "Renamed", NewPassword = null }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(oldHash, user.PasswordHash);
        Assert.Equal(PasswordVerificationResult.Success, _hasher.VerifyHashedPassword(user, user.PasswordHash, "OldPass123"));
    }

    [Fact]
    public async Task Update_WithNewPassword_ChangesPassword()
    {
        var user = GivenMember(5);

        await _service.UpdateAsync(AdminId, 5, new UpdateUserRequest { Email = user.Email, DisplayName = "Member", NewPassword = "NewPass123" }, default);

        Assert.Equal(PasswordVerificationResult.Success, _hasher.VerifyHashedPassword(user, user.PasswordHash, "NewPass123"));
    }

    // TC-U-08
    [Fact]
    public async Task Update_EmailOfAnotherUser_ReturnsConflict()
    {
        GivenMember(5);
        _users.Setup(u => u.EmailExistsAsync("member6@example.com", 5, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.UpdateAsync(AdminId, 5,
            new UpdateUserRequest { Email = "member6@example.com", DisplayName = "Member" }, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.EmailTaken, result.Title);
    }

    // TC-U-09, UM-03: including the Admin editing themself
    [Fact]
    public async Task Update_Admin_ReturnsForbidden()
    {
        var result = await _service.UpdateAsync(AdminId, AdminId,
            new UpdateUserRequest { Email = "admin@example.com", DisplayName = "Admin" }, default);

        Assert.Equal(ResultStatus.Forbidden, result.Status);
        Assert.Equal(ErrorMessages.AdminUserLocked, result.Title);
    }

    // TC-U-13, UM-04
    [Fact]
    public async Task Delete_UserWithTakenTask_ReturnsConflict()
    {
        GivenMember(5);
        _tasks.Setup(t => t.AnyTakenByUserAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.DeleteAsync(AdminId, 5, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.UserHasActiveTask, result.Title);
        _users.Verify(u => u.DeleteAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // TC-U-14
    [Fact]
    public async Task Delete_Admin_ReturnsForbidden()
    {
        var result = await _service.DeleteAsync(AdminId, AdminId, default);

        Assert.Equal(ResultStatus.Forbidden, result.Status);
        Assert.Equal(ErrorMessages.AdminUserLocked, result.Title);
    }

    // UM-04: no tasks, or only Done tasks
    [Fact]
    public async Task Delete_UserWithoutTakenTask_Succeeds()
    {
        GivenMember(5);

        var result = await _service.DeleteAsync(AdminId, 5, default);

        Assert.True(result.IsSuccess);
    }
}
