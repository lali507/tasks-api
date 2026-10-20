using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using TaskManager.Core.DTOs.Queries;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Results;
using TaskManager.Data;
using TaskManager.Data.Repositories;
using TaskManager.Service.Services;
using TaskManager.Tests.Infrastructure;

namespace TaskManager.Tests.Concurrency;

// Task 40: xmin (EN-04-R3) against real PostgreSQL
[Collection(DatabaseCollection.Name)]
public class ConcurrencyTests(DatabaseFixture fixture) : IAsyncLifetime
{
    private int _projectId;

    // A project of its own, so the seed tasks stay Open
    public async Task InitializeAsync()
    {
        await using var db = TestDatabase.CreateContext();
        var project = new Project { Name = $"Concurrency {Guid.NewGuid():N}", CreatedAt = DateTime.UtcNow };
        db.Projects.Add(project);
        await db.SaveChangesAsync();
        _projectId = project.Id;
    }

    public async Task DisposeAsync()
    {
        await using var db = TestDatabase.CreateContext();
        await db.TaskItems.Where(t => t.ProjectId == _projectId).ExecuteDeleteAsync();
        await db.Projects.Where(p => p.Id == _projectId).ExecuteDeleteAsync();
    }

    private async Task<int> NewOpenTaskAsync()
    {
        await using var db = TestDatabase.CreateContext();
        var task = new TaskItem { Title = "Race", ProjectId = _projectId, CreatedAt = DateTime.UtcNow };
        db.TaskItems.Add(task);
        await db.SaveChangesAsync();
        return task.Id;
    }

    // TC-C-06: both contexts read the same version; the second save fails on xmin
    [Fact]
    public async Task TwoContexts_SecondSaveGetsConcurrencyException()
    {
        var id = await NewOpenTaskAsync();
        await using var first = TestDatabase.CreateContext();
        await using var second = TestDatabase.CreateContext();
        var a = await first.TaskItems.SingleAsync(t => t.Id == id);
        var b = await second.TaskItems.SingleAsync(t => t.Id == id);

        a.Status = Status.Taken;
        a.TakenByUserId = 2;
        await first.SaveChangesAsync();

        b.Status = Status.Taken;
        b.TakenByUserId = 3;
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => second.SaveChangesAsync());
    }

    // TC-C-06 through the Repository: the exception becomes a result (GN-12-R2 / DA-04)
    [Fact]
    public async Task TwoRepositories_SecondUpdateReturnsConcurrencyConflict()
    {
        var id = await NewOpenTaskAsync();
        await using var first = TestDatabase.CreateContext();
        await using var second = TestDatabase.CreateContext();
        var repoA = new TaskItemRepository(first);
        var repoB = new TaskItemRepository(second);
        var a = (await repoA.FindAsync(id, default))!;
        var b = (await repoB.FindAsync(id, default))!;

        a.Status = Status.Taken;
        Assert.Equal(WriteResult.Success, await repoA.UpdateAsync(a, default));
        b.Status = Status.Taken;
        Assert.Equal(WriteResult.ConcurrencyConflict, await repoB.UpdateAsync(b, default));
    }

    // TC-C-07, GN-06-R3: two Members at the same moment -> one 200 and one 409, and a Warning when they really collided.
    // Whether both requests pass the Open check before either saves depends on timing, so several pairs are raced.
    [Fact]
    public async Task ParallelTakes_OneWinsOneGets409WithWarning()
    {
        var member2 = fixture.Api.CreateClientAs(2, Role.Member);
        var member3 = fixture.Api.CreateClientAs(3, Role.Member);
        var collisions = 0;

        for (var i = 0; i < 10; i++)
        {
            var id = await NewOpenTaskAsync();
            fixture.Api.Logs.Clear();
            using var start = new Barrier(2);
            Task<HttpResponseMessage> Take(HttpClient client) => Task.Run(() =>
            {
                start.SignalAndWait();
                return client.PutAsync($"/api/tasks/{id}/assignee", null);
            });

            var responses = await Task.WhenAll(Take(member2), Take(member3));

            Assert.Single(responses, r => r.StatusCode == HttpStatusCode.OK);
            var loser = Assert.Single(responses, r => r.StatusCode == HttpStatusCode.Conflict);
            await Problem.AssertAsync(loser, HttpStatusCode.Conflict, ErrorMessages.TaskAlreadyTaken);

            var warnings = fixture.Api.Logs.Entries.Count(e => e.Level == LogLevel.Warning && e.Message.Contains($"task {id} during take"));
            Assert.InRange(warnings, 0, 1);
            collisions += warnings;
        }

        Assert.True(collisions > 0, "No pair collided on xmin in 10 tries");
    }

    // TC-C-14: an Admin's update loses to a take that happened after the Admin read the task
    [Fact]
    public async Task AdminUpdate_TaskTakenMeanwhile_ReturnsTaskChangedWithWarning()
    {
        var id = await NewOpenTaskAsync();
        await using var adminDb = TestDatabase.CreateContext();
        await using var memberDb = TestDatabase.CreateContext();
        var logger = new Mock<ILogger<TaskService>>();
        // Right after the Admin's read, a Member takes the task through another context
        var tasks = new TakeAfterFind(new TaskItemRepository(adminDb), async () =>
        {
            var task = await memberDb.TaskItems.SingleAsync(t => t.Id == id);
            task.Status = Status.Taken;
            task.TakenByUserId = 2;
            await memberDb.SaveChangesAsync();
        });
        var service = new TaskService(tasks, new ProjectRepository(adminDb), new LabelRepository(adminDb), new UserRepository(adminDb), TimeProvider.System, logger.Object);

        var result = await service.UpdateAsync(1, id, new TaskRequest { Title = "Admin edit", ProjectId = _projectId }, default);

        Assert.Equal(ResultStatus.Conflict, result.Status);
        Assert.Equal(ErrorMessages.TaskChanged, result.Title);
        logger.Verify(l => l.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null,
            It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        await using var check = TestDatabase.CreateContext();
        var saved = await check.TaskItems.SingleAsync(t => t.Id == id);
        Assert.Equal("Race", saved.Title);
        Assert.Equal(Status.Taken, saved.Status);
    }

    // Runs a step between FindAsync and the save, to put a concurrent change exactly there
    private sealed class TakeAfterFind(ITaskItemRepository inner, Func<Task> afterFind) : ITaskItemRepository
    {
        public async Task<TaskItem?> FindAsync(int id, CancellationToken ct)
        {
            var task = await inner.FindAsync(id, ct);
            await afterFind();
            return task;
        }

        public Task<(List<TaskItem> Items, int TotalCount)> GetPagedAsync(TaskQuery query, int currentUserId, CancellationToken ct) => inner.GetPagedAsync(query, currentUserId, ct);
        public Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct) => inner.GetByIdAsync(id, ct);
        public Task<bool> AnyTakenByUserAsync(int userId, CancellationToken ct) => inner.AnyTakenByUserAsync(userId, ct);
        public Task<WriteResult> AddAsync(TaskItem task, CancellationToken ct) => inner.AddAsync(task, ct);
        public Task<WriteResult> UpdateAsync(TaskItem task, CancellationToken ct) => inner.UpdateAsync(task, ct);
        public Task<WriteResult> DeleteAsync(TaskItem task, CancellationToken ct) => inner.DeleteAsync(task, ct);
    }
}
