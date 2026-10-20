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
public class ProjectsControllerTests
{
    private const int AdminId = 1;
    private readonly MockedServicesFactory _api;
    private readonly HttpClient _admin;

    public ProjectsControllerTests(MockedServicesFactory api)
    {
        _api = api;
        _api.ResetMocks();
        _admin = api.CreateClientAs(AdminId, Role.Admin);
    }

    private static readonly ProjectResponse Project = new() { Id = 7, Name = "Site", TaskCount = 0 };

    [Fact]
    public async Task GetPaged_Returns200WithPage()
    {
        _api.Projects.Setup(p => p.GetPagedAsync(It.Is<PagingQuery>(q => q.Page == 2 && q.PageSize == 5), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<PagedResponse<ProjectResponse>>.Success(new() { Items = [Project], TotalCount = 6, Page = 2, PageSize = 5 }));

        var response = await _admin.GetAsync("/api/projects?page=2&pageSize=5");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var page = await response.Content.ReadFromJsonAsync<PagedResponse<ProjectResponse>>(Json.Options);
        Assert.Equal(6, page!.TotalCount);
    }

    // TC-P-02: the Service is not reached
    [Theory]
    [InlineData("pageSize=51")]
    [InlineData("page=0")]
    public async Task GetPaged_InvalidPaging_Returns400InvalidParameter(string query)
    {
        var response = await _admin.GetAsync($"/api/projects?{query}");

        await Problem.AssertAsync(response, HttpStatusCode.BadRequest, ErrorMessages.InvalidParameter);
        _api.Projects.Verify(p => p.GetPagedAsync(It.IsAny<PagingQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // TC-P-06
    [Fact]
    public async Task GetById_Missing_Returns404()
    {
        _api.Projects.Setup(p => p.GetByIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(ServiceResult<ProjectDetailsResponse>.NotFound());

        await Problem.AssertAsync(await _admin.GetAsync("/api/projects/99"), HttpStatusCode.NotFound, ErrorMessages.NotFound);
    }

    // AP-03: 201 with Location; the user id comes from the token
    [Fact]
    public async Task Create_Success_Returns201WithLocation()
    {
        _api.Projects.Setup(p => p.CreateAsync(AdminId, It.IsAny<ProjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<ProjectResponse>.Success(Project));

        var response = await _admin.PostAsJsonAsync("/api/projects", new { name = "Site" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/projects/7", response.Headers.Location?.ToString());
    }

    [Fact]
    public async Task Create_Conflict_Returns409WithTitle()
    {
        _api.Projects.Setup(p => p.CreateAsync(AdminId, It.IsAny<ProjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<ProjectResponse>.Conflict(ErrorMessages.ProjectNameTaken));

        var response = await _admin.PostAsJsonAsync("/api/projects", new { name = "Site" });

        await Problem.AssertAsync(response, HttpStatusCode.Conflict, ErrorMessages.ProjectNameTaken);
    }

    [Fact]
    public async Task Update_Success_Returns200()
    {
        _api.Projects.Setup(p => p.UpdateAsync(AdminId, 7, It.IsAny<ProjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<ProjectResponse>.Success(Project));

        Assert.Equal(HttpStatusCode.OK, (await _admin.PutAsJsonAsync("/api/projects/7", new { name = "Site" })).StatusCode);
    }

    // TC-P-13
    [Fact]
    public async Task Update_Missing_Returns404()
    {
        _api.Projects.Setup(p => p.UpdateAsync(AdminId, 99, It.IsAny<ProjectRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<ProjectResponse>.NotFound());

        await Problem.AssertAsync(await _admin.PutAsJsonAsync("/api/projects/99", new { name = "Site" }), HttpStatusCode.NotFound, ErrorMessages.NotFound);
    }

    [Fact]
    public async Task Delete_Success_Returns204()
    {
        _api.Projects.Setup(p => p.DeleteAsync(AdminId, 7, It.IsAny<CancellationToken>())).ReturnsAsync(ServiceResult.Success());

        Assert.Equal(HttpStatusCode.NoContent, (await _admin.DeleteAsync("/api/projects/7")).StatusCode);
    }

    [Fact]
    public async Task Delete_WithTasks_Returns409WithTitle()
    {
        _api.Projects.Setup(p => p.DeleteAsync(AdminId, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult.Conflict(ErrorMessages.ProjectHasTasks));

        await Problem.AssertAsync(await _admin.DeleteAsync("/api/projects/7"), HttpStatusCode.Conflict, ErrorMessages.ProjectHasTasks);
    }
}
