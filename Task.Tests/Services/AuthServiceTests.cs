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

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _users = new();
    private readonly PasswordHasher<User> _hasher = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _users.Setup(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.Success);
        _service = new AuthService(_users.Object, _hasher, new FixedTimeProvider());
    }

    private static RegisterRequest Register(string email = "new@example.com", string displayName = "New User") =>
        new() { Email = email, DisplayName = displayName, Password = "Password1" };

    // TC-A-02: an existing email, also in other letter case
    [Fact]
    public async Task Register_ExistingEmailInUpperCase_ReturnsConflict()
    {
        _users.Setup(u => u.EmailExistsAsync("member02@example.com", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.RegisterAsync(Register("MEMBER02@example.com"), default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.EmailTaken, result.Title);
    }

    // TC-A-04: spaces only, after Trim
    [Fact]
    public async Task Register_DisplayNameOfSpaces_ReturnsValidation()
    {
        var result = await _service.RegisterAsync(Register(displayName: "     "), default);

        Assert.Equal(ResultStatus.Validation, result.Status);
        Assert.Equal(ErrorMessages.InvalidData, result.Title);
        Assert.Contains(nameof(RegisterRequest.DisplayName), result.Errors!.Keys);
    }

    // TC-A-05, RL-01: trimmed, lowercased, always a Member, only the hash stored
    [Fact]
    public async Task Register_EmailWithSpacesAndCapitals_IsSavedTrimmedAndLowercase()
    {
        User? saved = null;
        _users.Setup(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => saved = user)
            .ReturnsAsync(WriteResult.Success);

        var result = await _service.RegisterAsync(Register("  New.User@Example.COM "), default);

        Assert.True(result.IsSuccess);
        Assert.Equal("new.user@example.com", saved!.Email);
        Assert.Equal(Role.Member, saved.Role);
        Assert.NotEqual("Password1", saved.PasswordHash);
        Assert.Equal(PasswordVerificationResult.Success, _hasher.VerifyHashedPassword(saved, saved.PasswordHash, "Password1"));
    }

    // TC-A-13, GN-12: the unique index catches a parallel registration
    [Fact]
    public async Task Register_DuplicateFromRepository_ReturnsConflictNot500()
    {
        _users.Setup(u => u.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.Duplicate);

        var result = await _service.RegisterAsync(Register(), default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.EmailTaken, result.Title);
    }

    // TC-A-08
    [Fact]
    public async Task Login_WrongPassword_ReturnsInvalidCredentials()
    {
        var user = new User { Id = 2, Email = "member02@example.com" };
        user.PasswordHash = _hasher.HashPassword(user, "Correct123");
        _users.Setup(u => u.GetByEmailAsync("member02@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.LoginAsync(new LoginRequest { Email = "member02@example.com", Password = "Wrong1234" }, default);

        Assert.Equal(ResultStatus.Unauthorized, result.Status);
        Assert.Equal(ErrorMessages.InvalidCredentials, result.Title);
    }

    // TC-A-09: the same answer as a wrong password
    [Fact]
    public async Task Login_UnknownEmail_ReturnsSameInvalidCredentials()
    {
        var result = await _service.LoginAsync(new LoginRequest { Email = "nobody@example.com", Password = "Wrong1234" }, default);

        Assert.Equal(ResultStatus.Unauthorized, result.Status);
        Assert.Equal(ErrorMessages.InvalidCredentials, result.Title);
    }

    [Fact]
    public async Task Login_CorrectPassword_ReturnsUser()
    {
        var user = new User { Id = 2, Email = "member02@example.com", DisplayName = "Member", Role = Role.Member };
        user.PasswordHash = _hasher.HashPassword(user, "Correct123");
        _users.Setup(u => u.GetByEmailAsync("member02@example.com", It.IsAny<CancellationToken>())).ReturnsAsync(user);

        var result = await _service.LoginAsync(new LoginRequest { Email = " Member02@Example.com ", Password = "Correct123" }, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Id);
    }
}
