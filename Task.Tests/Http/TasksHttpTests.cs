using System.Net;
using System.Net.Http.Json;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Enums;
using TaskManager.Core.Results;
using TaskManager.Tests.Infrastructure;

namespace TaskManager.Tests.Http;

[Collection(DatabaseCollection.Name)]
public class TasksHttpTests(DatabaseFixture fixture) : HttpTestBase(fixture)
{
    // TC-T-01: newest first
    [Fact]
    public async Task GetPaged_Default_TenNewestFirst()
    {
        var page = await ReadAsync<PagedResponse<TaskResponse>>(await Member().GetAsync("/api/tasks"));

        Assert.Equal(100, page.TotalCount);
        Assert.Equal(10, page.Items.Count);
        Assert.Equal(page.Items.Select(t => t.CreatedAt).OrderDescending(), page.Items.Select(t => t.CreatedAt));
    }

    // TC-T-02, FN-MB-03-R4
    [Fact]
    public async Task GetPaged_StatusAndProject_Combined()
    {
        var page = await ReadAsync<PagedResponse<TaskResponse>>(await Member().GetAsync("/api/tasks?status=Open&projectId=1&pageSize=50"));

        Assert.NotEmpty(page.Items);
        Assert.All(page.Items, t => { Assert.Equal(Status.Open, t.Status); Assert.Equal(1, t.ProjectId); });
    }

    // TC-T-03
    [Fact]
    public async Task GetPaged_UnknownStatus_Returns400() =>
        await Problem.AssertAsync(await Member().GetAsync("/api/tasks?status=Closed"), HttpStatusCode.BadRequest, ErrorMessages.InvalidParameter);

    // TC-T-04, FN-MB-03-R2
    [Fact]
    public async Task GetPaged_MissingProject_Empty()
    {
        var page = await ReadAsync<PagedResponse<TaskResponse>>(await Member().GetAsync("/api/tasks?projectId=9999"));

        Assert.Empty(page.Items);
        Assert.Equal(0, page.TotalCount);
    }

    // TC-T-05, TC-T-06, TC-C-01: mine=true is the caller's taken tasks; empty for an Admin
    [Fact]
    public async Task GetPaged_Mine_OnlyTasksTheCallerTook()
    {
        var project = await CreateProjectAsync();
        var task = await CreateTaskAsync(project.Id);
        var taken = await ReadAsync<TaskResponse>(await Member(4).PutAsync($"/api/tasks/{task.Id}/assignee", null));
        Assert.Equal(Status.Taken, taken.Status);
        Assert.Equal(4, taken.TakenByUserId);

        var mine = await ReadAsync<PagedResponse<TaskResponse>>(await Member(4).GetAsync("/api/tasks?mine=true"));
        var others = await ReadAsync<PagedResponse<TaskResponse>>(await Member(5).GetAsync("/api/tasks?mine=true"));
        var admin = await ReadAsync<PagedResponse<TaskResponse>>(await Admin.GetAsync("/api/tasks?mine=true"));

        Assert.Equal(task.Id, Assert.Single(mine.Items).Id);
        Assert.Empty(others.Items);
        Assert.Empty(admin.Items);
        await CleanUpAsync(project.Id, null, task);
    }

    // TC-T-07, DT-13-R1: labels, no xmin
    [Fact]
    public async Task GetById_Existing_WithLabelsWithoutXmin()
    {
        var response = await Member().GetAsync("/api/tasks/1");
        var task = await ReadAsync<TaskResponse>(response);

        Assert.NotEmpty(task.Labels);
        Assert.DoesNotContain("xmin", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    // TC-T-08, TC-T-17, TC-T-20, TC-C-04, TC-C-13
    [Fact]
    public async Task Missing_Returns404Everywhere()
    {
        var body = new { title = "Whatever", projectId = 1 };
        await Problem.AssertAsync(await Member().GetAsync("/api/tasks/9999"), HttpStatusCode.NotFound, ErrorMessages.NotFound);
        await Problem.AssertAsync(await Admin.PutAsJsonAsync("/api/tasks/9999", body), HttpStatusCode.NotFound, ErrorMessages.NotFound);
        await Problem.AssertAsync(await Admin.DeleteAsync("/api/tasks/9999"), HttpStatusCode.NotFound, ErrorMessages.NotFound);
        await Problem.AssertAsync(await Member().PutAsync("/api/tasks/9999/assignee", null), HttpStatusCode.NotFound, ErrorMessages.NotFound);
        await Problem.AssertAsync(await Member().PutAsync("/api/tasks/9999/completion", null), HttpStatusCode.NotFound, ErrorMessages.NotFound);
    }

    // TC-T-09, TC-T-15, TC-T-18: create Open with labels, replace the labels, delete
    [Fact]
    public async Task CreateUpdateDelete_FullCycle()
    {
        var project = await CreateProjectAsync();
        var response = await Admin.PostAsJsonAsync("/api/tasks", new { title = "Cycle", projectId = project.Id, labelIds = new[] { 1, 2 } });
        var task = await ReadAsync<TaskResponse>(response, HttpStatusCode.Created);
        Assert.Equal($"/api/tasks/{task.Id}", response.Headers.Location?.ToString());
        Assert.Equal(Status.Open, task.Status);
        Assert.Equal([1, 2], task.Labels.Select(l => l.Id).Order());

        var updated = await ReadAsync<TaskResponse>(await Admin.PutAsJsonAsync($"/api/tasks/{task.Id}", new { title = "Cycle 2", projectId = project.Id, labelIds = new[] { 3 } }));
        Assert.Equal(3, Assert.Single(updated.Labels).Id);

        await CleanUpAsync(project.Id, null, task);
    }

    // TC-T-10
    [Fact]
    public async Task Create_MissingProject_Returns400OnProjectId()
    {
        var problem = await Problem.AssertAsync(await Admin.PostAsJsonAsync("/api/tasks", new { title = "No project", projectId = 9999 }), HttpStatusCode.BadRequest, ErrorMessages.InvalidData);
        Assert.Contains("ProjectId", problem.Errors!.Keys);
    }

    // TC-T-13, TC-C-03, TC-C-12: the wrong role
    [Fact]
    public async Task WrongRole_Returns403()
    {
        await Problem.AssertAsync(await Member().PostAsJsonAsync("/api/tasks", new { title = "Nope", projectId = 1 }), HttpStatusCode.Forbidden, ErrorMessages.Forbidden);
        await Problem.AssertAsync(await Admin.PutAsync("/api/tasks/1/assignee", null), HttpStatusCode.Forbidden, ErrorMessages.Forbidden);
        await Problem.AssertAsync(await Admin.PutAsync("/api/tasks/1/completion", null), HttpStatusCode.Forbidden, ErrorMessages.Forbidden);
    }

    // TC-C-05
    [Fact]
    public async Task Take_NoToken_Returns401() =>
        await Problem.AssertAsync(await Anonymous.PutAsync("/api/tasks/1/assignee", null), HttpStatusCode.Unauthorized, ErrorMessages.LoginRequired);

    // ST-01 end to end: take, other blocked, Taken not deletable, complete, Done not updatable, Done deletable
    [Fact]
    public async Task Lifecycle_FollowsSt01()
    {
        var project = await CreateProjectAsync();
        var task = await CreateTaskAsync(project.Id);

        await ReadAsync<TaskResponse>(await Member(6).PutAsync($"/api/tasks/{task.Id}/assignee", null));
        await Problem.AssertAsync(await Member(7).PutAsync($"/api/tasks/{task.Id}/assignee", null), HttpStatusCode.Conflict, ErrorMessages.TaskAlreadyTaken);
        await Problem.AssertAsync(await Admin.DeleteAsync($"/api/tasks/{task.Id}"), HttpStatusCode.Conflict, ErrorMessages.TaskTakenCannotDelete);
        await Problem.AssertAsync(await Member(7).PutAsync($"/api/tasks/{task.Id}/completion", null), HttpStatusCode.Forbidden, ErrorMessages.OnlyTakerCanComplete);

        var done = await ReadAsync<TaskResponse>(await Member(6).PutAsync($"/api/tasks/{task.Id}/completion", null));
        Assert.Equal(Status.Done, done.Status);
        Assert.NotNull(done.CompletedAt);

        await Problem.AssertAsync(await Member(6).PutAsync($"/api/tasks/{task.Id}/completion", null), HttpStatusCode.Conflict, ErrorMessages.TaskNotTaken);
        await Problem.AssertAsync(await Admin.PutAsJsonAsync($"/api/tasks/{task.Id}", new { title = "Late", projectId = project.Id }), HttpStatusCode.Conflict, ErrorMessages.TaskCompleted);
        await CleanUpAsync(project.Id, null, task);
    }
}
