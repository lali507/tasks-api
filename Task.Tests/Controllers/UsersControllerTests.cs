using System.Net;
using System.Net.Http.Json;
using Moq;
using TaskManager.Core.DTOs.Queries;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Enums;
using TaskManager.Core.Results;
using TaskManager.Tests.Infrastructure;

namespace TaskManager.Tests.Controllers;

[Collection(ControllerCollection.Name)]
public class UsersControllerTests
{
    private const int AdminId = 1;
    private readonly MockedServicesFactory _api;
    private readonly HttpClient _admin;

    public UsersControllerTests(MockedServicesFactory api)
    {
        _api = api;
        _api.ResetMocks();
        _admin = api.CreateClientAs(AdminId, Role.Admin);
    }

    private static readonly UserResponse User5 = new() { Id = 5, Email = "member05@example.com", DisplayName = "M", Role = Role.Member };

    [Fact]
    public async Task GetPaged_Returns200()
    {
        _api.Users.Setup(u => u.GetPagedAsync(It.IsAny<PagingQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<PagedResponse<UserResponse>>.Success(new() { Items = [User5], TotalCount = 1, Page = 1, PageSize = 10 }));

        var response = await _admin.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.DoesNotContain("passwordHash", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Create_Success_Returns201WithLocation()
    {
        _api.Users.Setup(u => u.CreateAsync(AdminId, It.IsAny<CreateUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<UserResponse>.Success(User5));

        var response = await _admin.PostAsJsonAsync("/api/users", new { email = "member05@example.com", displayName = "M5", password = "Password1" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/users/5", response.Headers.Location?.ToString());
    }

    // TC-U-05
    [Fact]
    public async Task Create_InvalidPassword_Returns400OnPassword()
    {
        var response = await _admin.PostAsJsonAsync("/api/users", new { email = "member05@example.com", displayName = "M5", password = "short" });

        var problem = await Problem.AssertAsync(response, HttpStatusCode.BadRequest, ErrorMessages.InvalidData);
        Assert.Contains("Password", problem.Errors!.Keys);
    }

    [Fact]
    public async Task Update_Success_Returns200()
    {
        _api.Users.Setup(u => u.UpdateAsync(AdminId, 5, It.IsAny<UpdateUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<UserResponse>.Success(User5));

        Assert.Equal(HttpStatusCode.OK, (await _admin.PutAsJsonAsync("/api/users/5", new { email = "member05@example.com", displayName = "M5" })).StatusCode);
    }

    // TC-U-10
    [Fact]
    public async Task Update_Missing_Returns404()
    {
        _api.Users.Setup(u => u.UpdateAsync(AdminId, 99, It.IsAny<UpdateUserRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<UserResponse>.NotFound());

        var response = await _admin.PutAsJsonAsync("/api/users/99", new { email = "x@example.com", displayName = "X5" });

        await Problem.AssertAsync(response, HttpStatusCode.NotFound, ErrorMessages.NotFound);
    }

    // UM-03: the Service's 403 keeps its own title
    [Fact]
    public async Task Delete_Admin_Returns403WithTitle()
    {
        _api.Users.Setup(u => u.DeleteAsync(AdminId, AdminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult.Forbidden(ErrorMessages.AdminUserLocked));

        await Problem.AssertAsync(await _admin.DeleteAsync($"/api/users/{AdminId}"), HttpStatusCode.Forbidden, ErrorMessages.AdminUserLocked);
    }

    [Fact]
    public async Task Delete_Success_Returns204()
    {
        _api.Users.Setup(u => u.DeleteAsync(AdminId, 5, It.IsAny<CancellationToken>())).ReturnsAsync(ServiceResult.Success());

        Assert.Equal(HttpStatusCode.NoContent, (await _admin.DeleteAsync("/api/users/5")).StatusCode);
    }

    // TC-U-15
    [Fact]
    public async Task Delete_Missing_Returns404()
    {
        _api.Users.Setup(u => u.DeleteAsync(AdminId, 99, It.IsAny<CancellationToken>())).ReturnsAsync(ServiceResult.NotFound());

        await Problem.AssertAsync(await _admin.DeleteAsync("/api/users/99"), HttpStatusCode.NotFound, ErrorMessages.NotFound);
    }
}
