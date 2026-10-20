using System.Net;
using System.Net.Http.Json;
using Moq;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Enums;
using TaskManager.Core.Results;
using TaskManager.Tests.Infrastructure;

namespace TaskManager.Tests.Controllers;

[Collection(ControllerCollection.Name)]
public class AuthControllerTests
{
    private readonly MockedServicesFactory _api;
    private readonly HttpClient _client;

    public AuthControllerTests(MockedServicesFactory api)
    {
        _api = api;
        _api.ResetMocks();
        _client = api.CreateClient();
    }

    private static readonly UserResponse Member = new() { Id = 40, Email = "new@example.com", DisplayName = "New", Role = Role.Member };

    // FN-GU-01: 201 with Location and a token
    [Fact]
    public async Task Register_Success_Returns201WithToken()
    {
        _api.Auth.Setup(a => a.RegisterAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<UserResponse>.Success(Member));

        var response = await _client.PostAsJsonAsync("/api/auth/register", new { email = "new@example.com", displayName = "New", password = "Password1" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/users/40", response.Headers.Location?.ToString());
        var auth = await response.Content.ReadFromJsonAsync<AuthResponse>(Json.Options);
        Assert.False(string.IsNullOrEmpty(auth!.Token));
        Assert.Equal("New", auth.DisplayName);
        Assert.Equal(Role.Member, auth.Role);
    }

    // TC-A-03: 7 characters / no digit; the Service is not reached
    [Theory]
    [InlineData("Abcdef1")]
    [InlineData("Abcdefgh")]
    public async Task Register_InvalidPassword_Returns400OnPassword(string password)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/register", new { email = "new@example.com", displayName = "New", password });

        var problem = await Problem.AssertAsync(response, HttpStatusCode.BadRequest, ErrorMessages.InvalidData);
        Assert.Contains("Password", problem.Errors!.Keys);
        _api.Auth.Verify(a => a.RegisterAsync(It.IsAny<RegisterRequest>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // TC-A-10
    [Fact]
    public async Task Login_MissingField_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new { email = "admin@example.com" });

        var problem = await Problem.AssertAsync(response, HttpStatusCode.BadRequest, ErrorMessages.InvalidData);
        Assert.Contains("Password", problem.Errors!.Keys);
    }

    // FN-GU-02: the Service's 401 keeps its own title
    [Fact]
    public async Task Login_InvalidCredentials_Returns401WithTitle()
    {
        _api.Auth.Setup(a => a.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<UserResponse>.Unauthorized(ErrorMessages.InvalidCredentials));

        var response = await _client.PostAsJsonAsync("/api/auth/login", new { email = "admin@example.com", password = "Wrong1234" });

        await Problem.AssertAsync(response, HttpStatusCode.Unauthorized, ErrorMessages.InvalidCredentials);
    }

    [Fact]
    public async Task Login_Success_Returns200WithToken()
    {
        _api.Auth.Setup(a => a.LoginAsync(It.IsAny<LoginRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<UserResponse>.Success(Member));

        var response = await _client.PostAsJsonAsync("/api/auth/login", new { email = "new@example.com", password = "Password1" });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(string.IsNullOrEmpty((await response.Content.ReadFromJsonAsync<AuthResponse>(Json.Options))!.Token));
    }
}
