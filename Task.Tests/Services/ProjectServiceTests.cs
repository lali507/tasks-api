using Moq;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Results;
using TaskManager.Service.Services;
using TaskManager.Tests.Infrastructure;

namespace TaskManager.Tests.Services;

public class ProjectServiceTests
{
    private const int AdminId = 1;
    private readonly Mock<IProjectRepository> _projects = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly ProjectService _service;

    public ProjectServiceTests()
    {
        _users.Setup(u => u.ExistsAsync(AdminId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _projects.Setup(p => p.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.Success);
        _projects.Setup(p => p.UpdateAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.Success);
        _service = new ProjectService(_projects.Object, _users.Object, new FixedTimeProvider());
    }

    private void GivenProject(int id, string name) =>
        _projects.Setup(p => p.FindAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(new Project { Id = id, Name = name });

    // TC-P-08: names are compared case-insensitively by the database (citext); the Service passes the trimmed name
    [Fact]
    public async Task Create_ExistingName_ReturnsConflict()
    {
        _projects.Setup(p => p.NameExistsAsync("Company Site", null, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.CreateAsync(AdminId, new ProjectRequest { Name = "  Company Site " }, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.ProjectNameTaken, result.Title);
    }

    // TC-P-09: one character after Trim
    [Fact]
    public async Task Create_OneCharacterAfterTrim_ReturnsValidation()
    {
        var result = await _service.CreateAsync(AdminId, new ProjectRequest { Name = "  a  " }, default);

        Assert.Equal(ResultStatus.Validation, result.Status);
        Assert.Contains(nameof(ProjectRequest.Name), result.Errors!.Keys);
    }

    // GN-12: a parallel request with the same name
    [Fact]
    public async Task Create_DuplicateFromRepository_ReturnsConflict()
    {
        _projects.Setup(p => p.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.Duplicate);

        var result = await _service.CreateAsync(AdminId, new ProjectRequest { Name = "New Project" }, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
    }

    // GN-09-R1: the user in the token was deleted
    [Fact]
    public async Task Create_DeletedUser_ReturnsUnauthorized()
    {
        var result = await _service.CreateAsync(99, new ProjectRequest { Name = "New Project" }, default);

        Assert.Equal(ResultStatus.Unauthorized, result.Status);
        Assert.Equal(ErrorMessages.LoginRequired, result.Title);
    }

    // TC-P-11: the project's own name is excluded from the check
    [Fact]
    public async Task Update_SameNameSameProject_Succeeds()
    {
        GivenProject(5, "Company Site");

        var result = await _service.UpdateAsync(AdminId, 5, new ProjectRequest { Name = "Company Site" }, default);

        Assert.True(result.IsSuccess);
        _projects.Verify(p => p.NameExistsAsync("Company Site", 5, It.IsAny<CancellationToken>()));
    }

    // TC-P-12
    [Fact]
    public async Task Update_NameOfAnotherProject_ReturnsConflict()
    {
        GivenProject(5, "Company Site");
        _projects.Setup(p => p.NameExistsAsync("Mobile App", 5, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.UpdateAsync(AdminId, 5, new ProjectRequest { Name = "Mobile App" }, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.ProjectNameTaken, result.Title);
    }

    // TC-P-15, RE-01-R1
    [Fact]
    public async Task Delete_ProjectWithTasks_ReturnsConflict()
    {
        GivenProject(5, "Company Site");
        _projects.Setup(p => p.HasTasksAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(true);

        var result = await _service.DeleteAsync(AdminId, 5, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.ProjectHasTasks, result.Title);
        _projects.Verify(p => p.DeleteAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
