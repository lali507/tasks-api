using System.Net;
using System.Net.Http.Json;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Results;
using TaskManager.Tests.Infrastructure;

namespace TaskManager.Tests.Http;

[Collection(DatabaseCollection.Name)]
public class ProjectsHttpTests(DatabaseFixture fixture) : HttpTestBase(fixture)
{
    // TC-P-01
    [Fact]
    public async Task GetPaged_Default_FirstPageSortedByName()
    {
        var page = await ReadAsync<PagedResponse<ProjectResponse>>(await Member().GetAsync("/api/projects"));

        Assert.Equal(10, page.TotalCount);
        Assert.Equal(1, page.Page);
        Assert.Equal(10, page.Items.Count);
        Assert.Equal(page.Items.Select(p => p.Name).Order(StringComparer.Ordinal), page.Items.Select(p => p.Name));
    }

    // TC-P-03, GN-02-R1
    [Fact]
    public async Task GetPaged_PageAfterLast_EmptyWithTotalCount()
    {
        var page = await ReadAsync<PagedResponse<ProjectResponse>>(await Member().GetAsync("/api/projects?page=5"));

        Assert.Empty(page.Items);
        Assert.Equal(10, page.TotalCount);
    }

    // TC-P-02
    [Theory]
    [InlineData("pageSize=51")]
    [InlineData("page=0")]
    public async Task GetPaged_InvalidPaging_Returns400(string query) =>
        await Problem.AssertAsync(await Member().GetAsync($"/api/projects?{query}"), HttpStatusCode.BadRequest, ErrorMessages.InvalidParameter);

    // TC-P-04
    [Fact]
    public async Task GetPaged_NoToken_Returns401() =>
        await Problem.AssertAsync(await Anonymous.GetAsync("/api/projects"), HttpStatusCode.Unauthorized, ErrorMessages.LoginRequired);

    // TC-P-05: tasks with labels, oldest first
    [Fact]
    public async Task GetById_Existing_TasksWithLabelsByCreatedAt()
    {
        var project = await ReadAsync<ProjectDetailsResponse>(await Member().GetAsync("/api/projects/1"));

        Assert.Equal(10, project.TaskCount);
        Assert.Equal(10, project.Tasks.Count);
        Assert.Equal(project.Tasks.Select(t => t.CreatedAt).Order(), project.Tasks.Select(t => t.CreatedAt));
        Assert.Contains(project.Tasks, t => t.Labels.Count > 0);
    }

    // TC-P-06
    [Fact]
    public async Task GetById_Missing_Returns404() =>
        await Problem.AssertAsync(await Member().GetAsync("/api/projects/9999"), HttpStatusCode.NotFound, ErrorMessages.NotFound);

    // TC-P-07, TC-P-14
    [Fact]
    public async Task CreateThenDelete_Returns201ThenNoContent()
    {
        var response = await Admin.PostAsJsonAsync("/api/projects", new { name = Unique("Project") });
        var project = await ReadAsync<ProjectResponse>(response, HttpStatusCode.Created);

        Assert.Equal($"/api/projects/{project.Id}", response.Headers.Location?.ToString());
        Assert.Equal(0, project.TaskCount);
        Assert.Equal(HttpStatusCode.NoContent, (await Admin.DeleteAsync($"/api/projects/{project.Id}")).StatusCode);
    }

    // TC-P-08: other letter case (the seed names are Hebrew, which has none, so a Latin name is created)
    [Fact]
    public async Task Create_ExistingNameOtherCase_Returns409()
    {
        var project = await CreateProjectAsync();

        await Problem.AssertAsync(await Admin.PostAsJsonAsync("/api/projects", new { name = " " + project.Name.ToUpperInvariant() }),
            HttpStatusCode.Conflict, ErrorMessages.ProjectNameTaken);

        await CleanUpAsync(projectId: project.Id);
    }

    // TC-P-10, TC-P-16
    [Fact]
    public async Task MemberWrites_Return403()
    {
        await Problem.AssertAsync(await Member().PostAsJsonAsync("/api/projects", new { name = "Nope" }), HttpStatusCode.Forbidden, ErrorMessages.Forbidden);
        await Problem.AssertAsync(await Member().DeleteAsync("/api/projects/1"), HttpStatusCode.Forbidden, ErrorMessages.Forbidden);
    }

    // TC-P-12
    [Fact]
    public async Task Update_NameOfAnotherProject_Returns409()
    {
        var other = (await ReadAsync<ProjectDetailsResponse>(await Admin.GetAsync("/api/projects/2"))).Name;

        await Problem.AssertAsync(await Admin.PutAsJsonAsync("/api/projects/1", new { name = other }), HttpStatusCode.Conflict, ErrorMessages.ProjectNameTaken);
    }

    // TC-P-13
    [Fact]
    public async Task Update_Missing_Returns404() =>
        await Problem.AssertAsync(await Admin.PutAsJsonAsync("/api/projects/9999", new { name = "Whatever" }), HttpStatusCode.NotFound, ErrorMessages.NotFound);

    // TC-P-15
    [Fact]
    public async Task Delete_ProjectWithTasks_Returns409() =>
        await Problem.AssertAsync(await Admin.DeleteAsync("/api/projects/1"), HttpStatusCode.Conflict, ErrorMessages.ProjectHasTasks);
}
