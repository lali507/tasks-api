using System.Net;
using System.Net.Http.Json;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Enums;
using TaskManager.Tests.Infrastructure;

namespace TaskManager.Tests.Http;

// HTTP level (document 6): full requests against the in-memory API and the test database.
// Seed: user 1 is the Admin, users 2–30 are Members (password Demo1234!N). Tests that add data remove it, so the seed counts hold.
public abstract class HttpTestBase(DatabaseFixture fixture)
{
    protected const int AdminId = 1;

    protected ApiFactory Api => fixture.Api;
    protected HttpClient Anonymous => Api.CreateClient();
    protected HttpClient Admin => Api.CreateClientAs(AdminId, Role.Admin, "admin@example.com");
    protected HttpClient Member(int id = 2) => Api.CreateClientAs(id, Role.Member, $"member{id:00}@example.com");

    protected static string Unique(string prefix) => $"{prefix} {Guid.NewGuid().ToString("N")[..8]}";

    protected static async Task<T> ReadAsync<T>(HttpResponseMessage response, HttpStatusCode expected = HttpStatusCode.OK)
    {
        Assert.Equal(expected, response.StatusCode);
        return (await response.Content.ReadFromJsonAsync<T>(Json.Options))!;
    }

    protected async Task<ProjectResponse> CreateProjectAsync() =>
        await ReadAsync<ProjectResponse>(await Admin.PostAsJsonAsync("/api/projects", new { name = Unique("Project") }), HttpStatusCode.Created);

    protected async Task<LabelResponse> CreateLabelAsync() =>
        await ReadAsync<LabelResponse>(await Admin.PostAsJsonAsync("/api/labels", new { name = Unique("L") }), HttpStatusCode.Created);

    protected async Task<TaskResponse> CreateTaskAsync(int projectId, params int[] labelIds) =>
        await ReadAsync<TaskResponse>(await Admin.PostAsJsonAsync("/api/tasks", new { title = Unique("Task"), projectId, labelIds }), HttpStatusCode.Created);

    // A task that was taken can only be deleted once it is Done (FN-AD-11)
    protected async Task CleanUpAsync(int? projectId = null, int? labelId = null, params TaskResponse[] tasks)
    {
        foreach (var task in tasks)
        {
            var current = await ReadAsync<TaskResponse>(await Admin.GetAsync($"/api/tasks/{task.Id}"));
            if (current.Status == Status.Taken)
                await Member(current.TakenByUserId!.Value).PutAsync($"/api/tasks/{task.Id}/completion", null);
            Assert.Equal(HttpStatusCode.NoContent, (await Admin.DeleteAsync($"/api/tasks/{task.Id}")).StatusCode);
        }
        if (labelId is not null)
            Assert.Equal(HttpStatusCode.NoContent, (await Admin.DeleteAsync($"/api/labels/{labelId}")).StatusCode);
        if (projectId is not null)
            Assert.Equal(HttpStatusCode.NoContent, (await Admin.DeleteAsync($"/api/projects/{projectId}")).StatusCode);
    }
}
