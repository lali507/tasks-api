using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Results;
using TaskManager.Service.Services;
using TaskManager.Tests.Infrastructure;

namespace TaskManager.Tests.Services;

public class TaskServiceTests
{
    private const int AdminId = 1;
    private const int MemberId = 2;
    private const int OtherMemberId = 3;

    private readonly Mock<ITaskItemRepository> _tasks = new();
    private readonly Mock<IProjectRepository> _projects = new();
    private readonly Mock<ILabelRepository> _labels = new();
    private readonly Mock<IUserRepository> _users = new();
    private readonly Mock<ILogger<TaskService>> _logger = new();
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _users.Setup(u => u.ExistsAsync(It.IsIn(AdminId, MemberId, OtherMemberId), It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _projects.Setup(p => p.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _labels.Setup(l => l.GetByIdsAsync(It.IsAny<IEnumerable<int>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((IEnumerable<int> ids, CancellationToken _) => ids.Where(id => id <= 10).Select(id => new Label { Id = id, Name = $"L{id}" }).ToList());
        _tasks.Setup(t => t.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.Success);
        _tasks.Setup(t => t.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.Success);
        _tasks.Setup(t => t.DeleteAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.Success);
        _service = new TaskService(_tasks.Object, _projects.Object, _labels.Object, _users.Object, new FixedTimeProvider(), _logger.Object);
    }

    // FindAsync returns the tracked task; GetByIdAsync re-reads it with the project for the response
    private TaskItem GivenTask(int id, Status status, int? takenBy = null)
    {
        var task = new TaskItem { Id = id, Title = "Task", Status = status, TakenByUserId = takenBy, ProjectId = 1, Project = new Project { Id = 1, Name = "P" } };
        _tasks.Setup(t => t.FindAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        _tasks.Setup(t => t.GetByIdAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(task);
        return task;
    }

    private static TaskRequest Request(int projectId = 1, List<int>? labelIds = null) =>
        new() { Title = "A task", ProjectId = projectId, LabelIds = labelIds };

    // TC-T-10, FN-AD-09-R1: an id in the body that does not exist is 400, not 404
    [Fact]
    public async Task Create_MissingProject_ReturnsValidationOnProjectId()
    {
        var result = await _service.CreateAsync(AdminId, Request(projectId: 999), default);

        Assert.Equal(ResultStatus.Validation, result.Status);
        Assert.Equal(ErrorMessages.InvalidData, result.Title);
        Assert.Contains(nameof(TaskRequest.ProjectId), result.Errors!.Keys);
    }

    // TC-T-11
    [Fact]
    public async Task Create_MissingLabel_ReturnsValidationOnLabelIds()
    {
        var result = await _service.CreateAsync(AdminId, Request(labelIds: [1, 999]), default);

        Assert.Equal(ResultStatus.Validation, result.Status);
        Assert.Contains(nameof(TaskRequest.LabelIds), result.Errors!.Keys);
    }

    // TC-T-12, FN-AD-09-R2: duplicates are ignored; a new task is Open
    [Fact]
    public async Task Create_DuplicateLabelIds_AttachesEachLabelOnce()
    {
        TaskItem? saved = null;
        _tasks.Setup(t => t.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Callback<TaskItem, CancellationToken>((task, _) => { task.Id = 50; task.Project = new Project { Name = "P" }; saved = task; })
            .ReturnsAsync(WriteResult.Success);
        _tasks.Setup(t => t.GetByIdAsync(50, It.IsAny<CancellationToken>())).ReturnsAsync(() => saved);

        var result = await _service.CreateAsync(AdminId, Request(labelIds: [2, 2, 3, 2]), default);

        Assert.True(result.IsSuccess);
        Assert.Equal([2, 3], saved!.Labels.Select(l => l.Id).Order());
        Assert.Equal(Status.Open, saved.Status);
        Assert.Null(saved.TakenByUserId);
    }

    // TC-T-14: an Admin update leaves Status and TakenByUserId as they were
    [Fact]
    public async Task Update_TakenTask_KeepsStatusAndTaker()
    {
        var task = GivenTask(7, Status.Taken, takenBy: MemberId);

        var result = await _service.UpdateAsync(AdminId, 7, Request(labelIds: [4]), default);

        Assert.True(result.IsSuccess);
        Assert.Equal(Status.Taken, task.Status);
        Assert.Equal(MemberId, task.TakenByUserId);
        Assert.Equal([4], task.Labels.Select(l => l.Id));
    }

    // TC-T-16
    [Fact]
    public async Task Update_DoneTask_ReturnsConflict()
    {
        GivenTask(7, Status.Done, takenBy: MemberId);

        var result = await _service.UpdateAsync(AdminId, 7, Request(), default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.TaskCompleted, result.Title);
    }

    // FN-AD-10: the task changed since it was read
    [Fact]
    public async Task Update_ConcurrencyConflict_ReturnsTaskChanged()
    {
        GivenTask(7, Status.Open);
        _tasks.Setup(t => t.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.ConcurrencyConflict);

        var result = await _service.UpdateAsync(AdminId, 7, Request(), default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.TaskChanged, result.Title);
    }

    // TC-T-19
    [Fact]
    public async Task Delete_TakenTask_ReturnsConflict()
    {
        GivenTask(7, Status.Taken, takenBy: MemberId);

        var result = await _service.DeleteAsync(AdminId, 7, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.TaskTakenCannotDelete, result.Title);
        _tasks.Verify(t => t.DeleteAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    // FN-AD-11-R1
    [Theory]
    [InlineData(Status.Open)]
    [InlineData(Status.Done)]
    public async Task Delete_OpenOrDoneTask_Succeeds(Status status)
    {
        GivenTask(7, status);

        var result = await _service.DeleteAsync(AdminId, 7, default);

        Assert.True(result.IsSuccess);
    }

    // TC-C-01
    [Fact]
    public async Task Take_OpenTask_BecomesTakenByUser()
    {
        var task = GivenTask(7, Status.Open);

        var result = await _service.TakeAsync(MemberId, 7, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(Status.Taken, task.Status);
        Assert.Equal(MemberId, task.TakenByUserId);
    }

    // TC-C-02
    [Theory]
    [InlineData(Status.Taken)]
    [InlineData(Status.Done)]
    public async Task Take_NotOpen_ReturnsAlreadyTaken(Status status)
    {
        GivenTask(7, status, takenBy: OtherMemberId);

        var result = await _service.TakeAsync(MemberId, 7, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.TaskAlreadyTaken, result.Title);
    }

    // TC-C-08, GN-06: the conflict is a result (not an exception) and one Warning is logged
    [Fact]
    public async Task Take_ConcurrencyConflict_ReturnsConflictAndLogsWarning()
    {
        GivenTask(7, Status.Open);
        _tasks.Setup(t => t.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>())).ReturnsAsync(WriteResult.ConcurrencyConflict);

        var result = await _service.TakeAsync(MemberId, 7, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.TaskAlreadyTaken, result.Title);
        _logger.Verify(l => l.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
    }

    // TC-C-09
    [Fact]
    public async Task Complete_ByTaker_BecomesDoneWithCompletedAt()
    {
        var task = GivenTask(7, Status.Taken, takenBy: MemberId);

        var result = await _service.CompleteAsync(MemberId, 7, default);

        Assert.True(result.IsSuccess);
        Assert.Equal(Status.Done, task.Status);
        Assert.Equal(FixedTimeProvider.Default.UtcDateTime, task.CompletedAt);
    }

    // TC-C-10
    [Fact]
    public async Task Complete_ByOtherMember_ReturnsForbidden()
    {
        GivenTask(7, Status.Taken, takenBy: OtherMemberId);

        var result = await _service.CompleteAsync(MemberId, 7, default);

        Assert.Equal(ResultStatus.Forbidden, result.Status);
        Assert.Equal(ErrorMessages.OnlyTakerCanComplete, result.Title);
    }

    // TC-C-11
    [Theory]
    [InlineData(Status.Open)]
    [InlineData(Status.Done)]
    public async Task Complete_NotTaken_ReturnsConflict(Status status)
    {
        GivenTask(7, status, takenBy: status == Status.Done ? MemberId : null);

        var result = await _service.CompleteAsync(MemberId, 7, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.TaskNotTaken, result.Title);
    }

    // GN-09-R1, UM-07
    [Fact]
    public async Task Take_DeletedUser_ReturnsUnauthorized()
    {
        GivenTask(7, Status.Open);

        var result = await _service.TakeAsync(99, 7, default);

        Assert.Equal(ResultStatus.Unauthorized, result.Status);
    }
}
