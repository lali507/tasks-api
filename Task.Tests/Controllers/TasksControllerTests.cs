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
public class TasksControllerTests
{
    private const int AdminId = 1;
    private const int MemberId = 2;
    private readonly MockedServicesFactory _api;
    private readonly HttpClient _admin;
    private readonly HttpClient _member;

    public TasksControllerTests(MockedServicesFactory api)
    {
        _api = api;
        _api.ResetMocks();
        _admin = api.CreateClientAs(AdminId, Role.Admin);
        _member = api.CreateClientAs(MemberId, Role.Member);
    }

    private static readonly TaskResponse Task7 = new() { Id = 7, Title = "Task", Status = Status.Open, ProjectId = 1, ProjectName = "P" };
    private static readonly object Body = new { title = "Task", projectId = 1, labelIds = new[] { 1, 2 } };

    // GN-02: the filters reach the Service, with the user id from the token for mine=true
    [Fact]
    public async Task GetPaged_PassesFiltersAndUser()
    {
        _api.Tasks.Setup(t => t.GetPagedAsync(MemberId,
                It.Is<TaskQuery>(q => q.Status == Status.Open && q.ProjectId == 3 && q.Mine && q.PageSize == 20),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<PagedResponse<TaskResponse>>.Success(new() { Items = [Task7], TotalCount = 1, Page = 1, PageSize = 20 }));

        var response = await _member.GetAsync("/api/tasks?status=Open&projectId=3&mine=true&pageSize=20");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // DT-00-R5: the status is a string
        Assert.Contains("\"status\":\"Open\"", await response.Content.ReadAsStringAsync());
    }

    // TC-T-03
    [Fact]
    public async Task GetPaged_UnknownStatus_Returns400InvalidParameter()
    {
        var problem = await Problem.AssertAsync(await _member.GetAsync("/api/tasks?status=Closed"), HttpStatusCode.BadRequest, ErrorMessages.InvalidParameter);
        Assert.Contains("Status", problem.Errors!.Keys);
    }

    // TC-T-08
    [Fact]
    public async Task GetById_Missing_Returns404()
    {
        _api.Tasks.Setup(t => t.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(ServiceResult<TaskResponse>.NotFound());

        await Problem.AssertAsync(await _member.GetAsync("/api/tasks/99"), HttpStatusCode.NotFound, ErrorMessages.NotFound);
    }

    [Fact]
    public async Task Create_Success_Returns201WithLocation()
    {
        _api.Tasks.Setup(t => t.CreateAsync(AdminId, It.Is<TaskRequest>(r => r.LabelIds!.Count == 2), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<TaskResponse>.Success(Task7));

        var response = await _admin.PostAsJsonAsync("/api/tasks", Body);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/tasks/7", response.Headers.Location?.ToString());
    }

    // The Service's field errors become errors in the ProblemDetails (TC-T-10)
    [Fact]
    public async Task Create_ServiceValidation_Returns400WithErrors()
    {
        _api.Tasks.Setup(t => t.CreateAsync(AdminId, It.IsAny<TaskRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<TaskResponse>.Validation(new Dictionary<string, string[]> { ["ProjectId"] = ["לא נמצא"] }));

        var problem = await Problem.AssertAsync(await _admin.PostAsJsonAsync("/api/tasks", Body), HttpStatusCode.BadRequest, ErrorMessages.InvalidData);
        Assert.Equal(["לא נמצא"], problem.Errors!["ProjectId"]);
    }

    [Fact]
    public async Task Update_Success_Returns200()
    {
        _api.Tasks.Setup(t => t.UpdateAsync(AdminId, 7, It.IsAny<TaskRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<TaskResponse>.Success(Task7));

        Assert.Equal(HttpStatusCode.OK, (await _admin.PutAsJsonAsync("/api/tasks/7", Body)).StatusCode);
    }

    // TC-T-17
    [Fact]
    public async Task Update_Missing_Returns404()
    {
        _api.Tasks.Setup(t => t.UpdateAsync(AdminId, 99, It.IsAny<TaskRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<TaskResponse>.NotFound());

        await Problem.AssertAsync(await _admin.PutAsJsonAsync("/api/tasks/99", Body), HttpStatusCode.NotFound, ErrorMessages.NotFound);
    }

    [Fact]
    public async Task Delete_Success_Returns204()
    {
        _api.Tasks.Setup(t => t.DeleteAsync(AdminId, 7, It.IsAny<CancellationToken>())).ReturnsAsync(ServiceResult.Success());

        Assert.Equal(HttpStatusCode.NoContent, (await _admin.DeleteAsync("/api/tasks/7")).StatusCode);
    }

    // TC-T-20
    [Fact]
    public async Task Delete_Missing_Returns404()
    {
        _api.Tasks.Setup(t => t.DeleteAsync(AdminId, 99, It.IsAny<CancellationToken>())).ReturnsAsync(ServiceResult.NotFound());

        await Problem.AssertAsync(await _admin.DeleteAsync("/api/tasks/99"), HttpStatusCode.NotFound, ErrorMessages.NotFound);
    }

    // FN-MB-06: no body; the user comes from the token
    [Fact]
    public async Task Take_Success_Returns200()
    {
        _api.Tasks.Setup(t => t.TakeAsync(MemberId, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<TaskResponse>.Success(Task7 with { Status = Status.Taken, TakenByUserId = MemberId }));

        Assert.Equal(HttpStatusCode.OK, (await _member.PutAsync("/api/tasks/7/assignee", null)).StatusCode);
    }

    // TC-C-04
    [Fact]
    public async Task Take_Missing_Returns404()
    {
        _api.Tasks.Setup(t => t.TakeAsync(MemberId, 99, It.IsAny<CancellationToken>())).ReturnsAsync(ServiceResult<TaskResponse>.NotFound());

        await Problem.AssertAsync(await _member.PutAsync("/api/tasks/99/assignee", null), HttpStatusCode.NotFound, ErrorMessages.NotFound);
    }

    [Fact]
    public async Task Take_AlreadyTaken_Returns409WithTitle()
    {
        _api.Tasks.Setup(t => t.TakeAsync(MemberId, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<TaskResponse>.Conflict(ErrorMessages.TaskAlreadyTaken));

        await Problem.AssertAsync(await _member.PutAsync("/api/tasks/7/assignee", null), HttpStatusCode.Conflict, ErrorMessages.TaskAlreadyTaken);
    }

    // The Service's 403 keeps its own title
    [Fact]
    public async Task Complete_ByOtherMember_Returns403WithTitle()
    {
        _api.Tasks.Setup(t => t.CompleteAsync(MemberId, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<TaskResponse>.Forbidden(ErrorMessages.OnlyTakerCanComplete));

        await Problem.AssertAsync(await _member.PutAsync("/api/tasks/7/completion", null), HttpStatusCode.Forbidden, ErrorMessages.OnlyTakerCanComplete);
    }

    // TC-C-13
    [Fact]
    public async Task Complete_Missing_Returns404()
    {
        _api.Tasks.Setup(t => t.CompleteAsync(MemberId, 99, It.IsAny<CancellationToken>())).ReturnsAsync(ServiceResult<TaskResponse>.NotFound());

        await Problem.AssertAsync(await _member.PutAsync("/api/tasks/99/completion", null), HttpStatusCode.NotFound, ErrorMessages.NotFound);
    }
}
