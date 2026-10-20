using System.Net;
using System.Net.Http.Json;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Results;
using TaskManager.Tests.Infrastructure;

namespace TaskManager.Tests.Http;

[Collection(DatabaseCollection.Name)]
public class LabelsHttpTests(DatabaseFixture fixture) : HttpTestBase(fixture)
{
    // TC-L-01: a plain array, all 10, by name
    [Fact]
    public async Task GetAll_Member_AllLabelsSortedByName()
    {
        var response = await Member().GetAsync("/api/labels");
        var labels = await ReadAsync<List<LabelResponse>>(response);

        Assert.StartsWith("[", await response.Content.ReadAsStringAsync());
        Assert.Equal(10, labels.Count);
        Assert.Equal(labels.Select(l => l.Name).Order(StringComparer.Ordinal), labels.Select(l => l.Name));
    }

    // TC-L-02
    [Fact]
    public async Task GetAll_NoToken_Returns401() =>
        await Problem.AssertAsync(await Anonymous.GetAsync("/api/labels"), HttpStatusCode.Unauthorized, ErrorMessages.LoginRequired);

    // TC-L-03, TC-L-04
    [Fact]
    public async Task Create_Returns201_ThenSameNameOtherCase409()
    {
        var response = await Admin.PostAsJsonAsync("/api/labels", new { name = Unique("Label") });
        var label = await ReadAsync<LabelResponse>(response, HttpStatusCode.Created);
        Assert.Equal($"/api/labels/{label.Id}", response.Headers.Location?.ToString());

        await Problem.AssertAsync(await Admin.PostAsJsonAsync("/api/labels", new { name = label.Name.ToUpperInvariant() }), HttpStatusCode.Conflict, ErrorMessages.LabelNameTaken);

        await CleanUpAsync(labelId: label.Id);
    }

    // TC-L-05
    [Fact]
    public async Task Create_NameOf31Characters_Returns400()
    {
        var problem = await Problem.AssertAsync(await Admin.PostAsJsonAsync("/api/labels", new { name = new string('a', 31) }), HttpStatusCode.BadRequest, ErrorMessages.InvalidData);
        Assert.Contains("Name", problem.Errors!.Keys);
    }

    // TC-L-06, TC-L-12
    [Fact]
    public async Task MemberWrites_Return403()
    {
        await Problem.AssertAsync(await Member().PostAsJsonAsync("/api/labels", new { name = "Nope" }), HttpStatusCode.Forbidden, ErrorMessages.Forbidden);
        await Problem.AssertAsync(await Member().DeleteAsync("/api/labels/1"), HttpStatusCode.Forbidden, ErrorMessages.Forbidden);
    }

    // TC-L-07: the new name shows on the linked tasks
    [Fact]
    public async Task Update_NewNameShowsOnTasks()
    {
        var project = await CreateProjectAsync();
        var label = await CreateLabelAsync();
        var task = await CreateTaskAsync(project.Id, label.Id);
        var newName = Unique("Renamed");

        await ReadAsync<LabelResponse>(await Admin.PutAsJsonAsync($"/api/labels/{label.Id}", new { name = newName }));

        var reread = await ReadAsync<TaskResponse>(await Member().GetAsync($"/api/tasks/{task.Id}"));
        Assert.Equal(newName, Assert.Single(reread.Labels).Name);
        await CleanUpAsync(project.Id, label.Id, task);
    }

    // TC-L-08
    [Fact]
    public async Task Update_NameOfAnotherLabel_Returns409()
    {
        var labels = await ReadAsync<List<LabelResponse>>(await Admin.GetAsync("/api/labels"));

        await Problem.AssertAsync(await Admin.PutAsJsonAsync($"/api/labels/{labels[0].Id}", new { name = labels[1].Name }), HttpStatusCode.Conflict, ErrorMessages.LabelNameTaken);
    }

    // TC-L-09, TC-L-11
    [Fact]
    public async Task Missing_Returns404()
    {
        await Problem.AssertAsync(await Admin.PutAsJsonAsync("/api/labels/9999", new { name = "Whatever" }), HttpStatusCode.NotFound, ErrorMessages.NotFound);
        await Problem.AssertAsync(await Admin.DeleteAsync("/api/labels/9999"), HttpStatusCode.NotFound, ErrorMessages.NotFound);
    }

    // TC-L-10, RE-03: the tasks stay, without the label
    [Fact]
    public async Task Delete_LinkedLabel_TasksRemainWithoutIt()
    {
        var project = await CreateProjectAsync();
        var label = await CreateLabelAsync();
        var other = await CreateLabelAsync();
        var task = await CreateTaskAsync(project.Id, label.Id, other.Id);

        Assert.Equal(HttpStatusCode.NoContent, (await Admin.DeleteAsync($"/api/labels/{label.Id}")).StatusCode);

        var reread = await ReadAsync<TaskResponse>(await Member().GetAsync($"/api/tasks/{task.Id}"));
        Assert.Equal(other.Id, Assert.Single(reread.Labels).Id);
        await CleanUpAsync(project.Id, other.Id, task);
    }
}
