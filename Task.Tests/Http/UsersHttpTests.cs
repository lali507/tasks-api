using System.Net;
using System.Net.Http.Json;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Enums;
using TaskManager.Core.Results;
using TaskManager.Tests.Infrastructure;

namespace TaskManager.Tests.Http;

[Collection(DatabaseCollection.Name)]
public class UsersHttpTests(DatabaseFixture fixture) : HttpTestBase(fixture)
{
    private async Task<UserResponse> CreateUserAsync(string password = "Password1") =>
        await ReadAsync<UserResponse>(
            await Admin.PostAsJsonAsync("/api/users", new { email = $"{Guid.NewGuid():N}@test.com", displayName = "Temp User", password }), HttpStatusCode.Created);

    private async Task<HttpStatusCode> LoginAsync(string email, string password) =>
        (await Anonymous.PostAsJsonAsync("/api/auth/login", new { email, password })).StatusCode;

    // TC-U-01: 10 by DisplayName, 30 in all, no PasswordHash
    [Fact]
    public async Task GetPaged_Admin_SortedByDisplayName()
    {
        var response = await Admin.GetAsync("/api/users");
        var page = await ReadAsync<PagedResponse<UserResponse>>(response);

        Assert.Equal(30, page.TotalCount);
        Assert.Equal(10, page.Items.Count);
        Assert.Equal(page.Items.Select(u => u.DisplayName).Order(StringComparer.Ordinal), page.Items.Select(u => u.DisplayName));
        Assert.DoesNotContain("passwordHash", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    // TC-U-02
    [Fact]
    public async Task GetPaged_Member_Returns403() =>
        await Problem.AssertAsync(await Member().GetAsync("/api/users"), HttpStatusCode.Forbidden, ErrorMessages.Forbidden);

    // TC-U-03, TC-U-04, TC-U-11
    [Fact]
    public async Task Create_IsMemberWithoutToken_ThenDuplicate409_ThenDelete204()
    {
        var response = await Admin.PostAsJsonAsync("/api/users", new { email = $"{Guid.NewGuid():N}@test.com", displayName = "Temp User", password = "Password1" });
        var user = await ReadAsync<UserResponse>(response, HttpStatusCode.Created);
        Assert.Equal($"/api/users/{user.Id}", response.Headers.Location?.ToString());
        Assert.Equal(Role.Member, user.Role);
        Assert.DoesNotContain("token", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);

        await Problem.AssertAsync(await Admin.PostAsJsonAsync("/api/users", new { email = user.Email.ToUpperInvariant(), displayName = "Again", password = "Password1" }),
            HttpStatusCode.Conflict, ErrorMessages.EmailTaken);

        Assert.Equal(HttpStatusCode.NoContent, (await Admin.DeleteAsync($"/api/users/{user.Id}")).StatusCode);
    }

    // TC-U-07: only the new password works
    [Fact]
    public async Task Update_WithNewPassword_OnlyNewPasswordWorks()
    {
        var user = await CreateUserAsync("OldPass123");

        await ReadAsync<UserResponse>(await Admin.PutAsJsonAsync($"/api/users/{user.Id}", new { email = user.Email, displayName = "Temp User", newPassword = "NewPass123" }));

        Assert.Equal(HttpStatusCode.OK, await LoginAsync(user.Email, "NewPass123"));
        Assert.Equal(HttpStatusCode.Unauthorized, await LoginAsync(user.Email, "OldPass123"));
        await Admin.DeleteAsync($"/api/users/{user.Id}");
    }

    // TC-U-06: without NewPassword the old one keeps working
    [Fact]
    public async Task Update_WithoutNewPassword_OldPasswordWorks()
    {
        var user = await CreateUserAsync("OldPass123");

        await ReadAsync<UserResponse>(await Admin.PutAsJsonAsync($"/api/users/{user.Id}", new { email = user.Email, displayName = "Renamed" }));

        Assert.Equal(HttpStatusCode.OK, await LoginAsync(user.Email, "OldPass123"));
        await Admin.DeleteAsync($"/api/users/{user.Id}");
    }

    // TC-U-09, TC-U-14
    [Fact]
    public async Task EditOrDeleteAdmin_Returns403()
    {
        await Problem.AssertAsync(await Admin.PutAsJsonAsync($"/api/users/{AdminId}", new { email = "admin@example.com", displayName = "Admin X" }),
            HttpStatusCode.Forbidden, ErrorMessages.AdminUserLocked);
        await Problem.AssertAsync(await Admin.DeleteAsync($"/api/users/{AdminId}"), HttpStatusCode.Forbidden, ErrorMessages.AdminUserLocked);
    }

    // TC-U-12, TC-U-13, UM-05: Taken blocks; once Done, the task stays Done without a user
    [Fact]
    public async Task Delete_WithTakenTask409_AfterDone204AndTaskKeepsDone()
    {
        var user = await CreateUserAsync();
        var asUser = Api.CreateClientAs(user.Id, Role.Member, user.Email);
        var project = await CreateProjectAsync();
        var task = await CreateTaskAsync(project.Id);
        await ReadAsync<TaskResponse>(await asUser.PutAsync($"/api/tasks/{task.Id}/assignee", null));

        await Problem.AssertAsync(await Admin.DeleteAsync($"/api/users/{user.Id}"), HttpStatusCode.Conflict, ErrorMessages.UserHasActiveTask);

        await ReadAsync<TaskResponse>(await asUser.PutAsync($"/api/tasks/{task.Id}/completion", null));
        Assert.Equal(HttpStatusCode.NoContent, (await Admin.DeleteAsync($"/api/users/{user.Id}")).StatusCode);

        var reread = await ReadAsync<TaskResponse>(await Admin.GetAsync($"/api/tasks/{task.Id}"));
        Assert.Equal(Status.Done, reread.Status);
        Assert.Null(reread.TakenByUserId);
        Assert.Null(reread.TakenByDisplayName);
        Assert.NotNull(reread.CompletedAt);
        await CleanUpAsync(project.Id, null, task);
    }

    // TC-U-16, UM-07: the token outlives the user, but writes are refused
    [Fact]
    public async Task DeletedUserToken_WriteReturns401()
    {
        var user = await CreateUserAsync();
        var asUser = Api.CreateClientAs(user.Id, Role.Member, user.Email);
        await Admin.DeleteAsync($"/api/users/{user.Id}");

        await Problem.AssertAsync(await asUser.PutAsync("/api/tasks/1/assignee", null), HttpStatusCode.Unauthorized, ErrorMessages.LoginRequired);
    }
}
