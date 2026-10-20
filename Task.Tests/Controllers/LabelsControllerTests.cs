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
public class LabelsControllerTests
{
    private const int AdminId = 1;
    private readonly MockedServicesFactory _api;
    private readonly HttpClient _admin;

    public LabelsControllerTests(MockedServicesFactory api)
    {
        _api = api;
        _api.ResetMocks();
        _admin = api.CreateClientAs(AdminId, Role.Admin);
    }

    // FN-MB-05-R1: a plain list, not a PagedResponse
    [Fact]
    public async Task GetAll_Returns200WithList()
    {
        _api.Labels.Setup(l => l.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<List<LabelResponse>>.Success([new() { Id = 1, Name = "Bug" }]));

        var response = await _admin.GetAsync("/api/labels");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Single((await response.Content.ReadFromJsonAsync<List<LabelResponse>>(Json.Options))!);
    }

    [Fact]
    public async Task Create_Success_Returns201WithLocation()
    {
        _api.Labels.Setup(l => l.CreateAsync(AdminId, It.IsAny<LabelRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<LabelResponse>.Success(new() { Id = 11, Name = "New" }));

        var response = await _admin.PostAsJsonAsync("/api/labels", new { name = "New" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/labels/11", response.Headers.Location?.ToString());
    }

    // TC-L-05
    [Fact]
    public async Task Create_NameOf31Characters_Returns400OnName()
    {
        var response = await _admin.PostAsJsonAsync("/api/labels", new { name = new string('a', 31) });

        var problem = await Problem.AssertAsync(response, HttpStatusCode.BadRequest, ErrorMessages.InvalidData);
        Assert.Contains("Name", problem.Errors!.Keys);
    }

    [Fact]
    public async Task Update_Success_Returns200()
    {
        _api.Labels.Setup(l => l.UpdateAsync(AdminId, 3, It.IsAny<LabelRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<LabelResponse>.Success(new() { Id = 3, Name = "Renamed" }));

        Assert.Equal(HttpStatusCode.OK, (await _admin.PutAsJsonAsync("/api/labels/3", new { name = "Renamed" })).StatusCode);
    }

    // TC-L-09
    [Fact]
    public async Task Update_Missing_Returns404()
    {
        _api.Labels.Setup(l => l.UpdateAsync(AdminId, 99, It.IsAny<LabelRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<LabelResponse>.NotFound());

        await Problem.AssertAsync(await _admin.PutAsJsonAsync("/api/labels/99", new { name = "Renamed" }), HttpStatusCode.NotFound, ErrorMessages.NotFound);
    }

    [Fact]
    public async Task Delete_Success_Returns204()
    {
        _api.Labels.Setup(l => l.DeleteAsync(AdminId, 3, It.IsAny<CancellationToken>())).ReturnsAsync(ServiceResult.Success());

        Assert.Equal(HttpStatusCode.NoContent, (await _admin.DeleteAsync("/api/labels/3")).StatusCode);
    }

    // TC-L-11
    [Fact]
    public async Task Delete_Missing_Returns404()
    {
        _api.Labels.Setup(l => l.DeleteAsync(AdminId, 99, It.IsAny<CancellationToken>())).ReturnsAsync(ServiceResult.NotFound());

        await Problem.AssertAsync(await _admin.DeleteAsync("/api/labels/99"), HttpStatusCode.NotFound, ErrorMessages.NotFound);
    }
}
