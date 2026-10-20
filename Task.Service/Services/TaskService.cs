using TaskManager.Core.DTOs.Queries;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Entities;
using TaskManager.Core.Enums;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Results;
using TaskManager.Core.Validation;
using TaskManager.Service.Mapping;
using TaskManager.Service.Validation;

namespace TaskManager.Service.Services;

// Writes check in this order: user exists (401) -> resource exists (404) -> fields (400) -> business rules (409)
public class TaskService(
    ITaskItemRepository tasks,
    IProjectRepository projects,
    ILabelRepository labels,
    IUserRepository users,
    TimeProvider time) : ITaskService
{
    // FN-MB-03, FN-AD-03: for an Admin, mine=true is always empty (RL-03)
    public async Task<ServiceResult<PagedResponse<TaskResponse>>> GetPagedAsync(int currentUserId, TaskQuery query, CancellationToken ct)
    {
        var (items, totalCount) = await tasks.GetPagedAsync(query, currentUserId, ct);
        return ServiceResult<PagedResponse<TaskResponse>>.Success(new PagedResponse<TaskResponse>
        {
            Items = items.Select(t => t.ToResponse()).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
        });
    }

    // FN-MB-04, FN-AD-04
    public async Task<ServiceResult<TaskResponse>> GetByIdAsync(int id, CancellationToken ct)
    {
        var task = await tasks.GetByIdAsync(id, ct);
        return task is null
            ? ServiceResult<TaskResponse>.NotFound()
            : ServiceResult<TaskResponse>.Success(task.ToResponse());
    }

    // FN-AD-09
    public async Task<ServiceResult<TaskResponse>> CreateAsync(int currentUserId, TaskRequest request, CancellationToken ct)
    {
        // GN-09-R1
        if (!await users.ExistsAsync(currentUserId, ct))
            return ServiceResult<TaskResponse>.Unauthorized();

        var (fields, errors) = await ValidateAsync(request, ct);
        if (errors.HasErrors)
            return ServiceResult<TaskResponse>.Validation(errors.ToDictionary());

        var task = new TaskItem
        {
            Title = fields.Title,
            Description = fields.Description,
            ProjectId = request.ProjectId,
            Status = Status.Open,
            CreatedAt = time.GetUtcNow().UtcDateTime,
            Labels = fields.Labels,
        };
        await tasks.AddAsync(task, ct);

        return ServiceResult<TaskResponse>.Success(await ToResponseAsync(task.Id, ct));
    }

    // FN-AD-10: Status and TakenByUserId stay unchanged
    public async Task<ServiceResult<TaskResponse>> UpdateAsync(int currentUserId, int id, TaskRequest request, CancellationToken ct)
    {
        if (!await users.ExistsAsync(currentUserId, ct))
            return ServiceResult<TaskResponse>.Unauthorized();

        var task = await tasks.FindAsync(id, ct);
        if (task is null)
            return ServiceResult<TaskResponse>.NotFound();

        var (fields, errors) = await ValidateAsync(request, ct);
        if (errors.HasErrors)
            return ServiceResult<TaskResponse>.Validation(errors.ToDictionary());

        if (task.Status == Status.Done)
            return ServiceResult<TaskResponse>.Conflict(ErrorMessages.TaskCompleted);

        task.Title = fields.Title;
        task.Description = fields.Description;
        task.ProjectId = request.ProjectId;
        // LabelIds replaces all existing labels
        task.Labels.Clear();
        foreach (var label in fields.Labels)
            task.Labels.Add(label);

        // EN-04-R3: the task changed (e.g. was taken) since FindAsync
        if (await tasks.UpdateAsync(task, ct) == WriteResult.ConcurrencyConflict)
            return ServiceResult<TaskResponse>.Conflict(ErrorMessages.TaskChanged);

        return ServiceResult<TaskResponse>.Success(await ToResponseAsync(id, ct));
    }

    // FN-AD-11: Open and Done may be deleted (FN-AD-11-R1)
    public async Task<ServiceResult> DeleteAsync(int currentUserId, int id, CancellationToken ct)
    {
        if (!await users.ExistsAsync(currentUserId, ct))
            return ServiceResult.Unauthorized();

        var task = await tasks.FindAsync(id, ct);
        if (task is null)
            return ServiceResult.NotFound();

        if (task.Status == Status.Taken)
            return ServiceResult.Conflict(ErrorMessages.TaskTakenCannotDelete);

        // Taking is the only change an Open task can go through after FindAsync
        if (await tasks.DeleteAsync(task, ct) == WriteResult.ConcurrencyConflict)
            return ServiceResult.Conflict(ErrorMessages.TaskTakenCannotDelete);

        return ServiceResult.Success();
    }

    // FN-MB-06: task 18
    public Task<ServiceResult<TaskResponse>> TakeAsync(int currentUserId, int id, CancellationToken ct) =>
        throw new NotImplementedException();

    // FN-MB-07: task 19
    public Task<ServiceResult<TaskResponse>> CompleteAsync(int currentUserId, int id, CancellationToken ct) =>
        throw new NotImplementedException();

    // FN-00-R2, FN-AD-09-R1: an id in the body that does not exist is a field error (400), not 404
    private async Task<(TaskFields Fields, FieldErrors Errors)> ValidateAsync(TaskRequest request, CancellationToken ct)
    {
        var errors = new FieldErrors();
        var title = errors.Text(nameof(request.Title), request.Title, min: 2, max: 100);
        var description = errors.OptionalText(nameof(request.Description), request.Description, max: 1000);

        if (!await projects.ExistsAsync(request.ProjectId, ct))
            errors.Add(nameof(request.ProjectId), ValidationMessages.NotFound);

        // FN-AD-09-R2: empty or null means no labels; duplicates are ignored
        var labelIds = (request.LabelIds ?? []).Distinct().ToList();
        var found = labelIds.Count == 0 ? [] : await labels.GetByIdsAsync(labelIds, ct);
        if (found.Count != labelIds.Count)
            errors.Add(nameof(request.LabelIds), ValidationMessages.NotFound);

        return (new TaskFields(title, description, found), errors);
    }

    // Re-reads with project, taker and labels for the response
    private async Task<TaskResponse> ToResponseAsync(int id, CancellationToken ct) =>
        (await tasks.GetByIdAsync(id, ct))!.ToResponse();

    private sealed record TaskFields(string Title, string? Description, List<Label> Labels);
}
