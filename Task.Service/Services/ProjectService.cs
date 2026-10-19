using TaskManager.Core.DTOs.Queries;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Entities;
using TaskManager.Core.Interfaces.Repositories;
using TaskManager.Core.Interfaces.Services;
using TaskManager.Core.Results;
using TaskManager.Service.Mapping;
using TaskManager.Service.Validation;

namespace TaskManager.Service.Services;

// Writes check in this order: user exists (401) -> resource exists (404) -> fields (400) -> business rules (409)
public class ProjectService(IProjectRepository projects, IUserRepository users, TimeProvider time) : IProjectService
{
    // FN-MB-01, FN-AD-01
    public async Task<ServiceResult<PagedResponse<ProjectResponse>>> GetPagedAsync(PagingQuery query, CancellationToken ct)
    {
        var (items, totalCount) = await projects.GetPagedAsync(query.Page, query.PageSize, ct);
        return ServiceResult<PagedResponse<ProjectResponse>>.Success(new PagedResponse<ProjectResponse>
        {
            Items = items.Select(i => i.Project.ToResponse(i.TaskCount)).ToList(),
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize,
        });
    }

    // FN-MB-02, FN-AD-02
    public async Task<ServiceResult<ProjectDetailsResponse>> GetByIdAsync(int id, CancellationToken ct)
    {
        var project = await projects.GetDetailsAsync(id, ct);
        return project is null
            ? ServiceResult<ProjectDetailsResponse>.NotFound()
            : ServiceResult<ProjectDetailsResponse>.Success(project.ToDetailsResponse());
    }

    // FN-AD-06
    public async Task<ServiceResult<ProjectResponse>> CreateAsync(int currentUserId, ProjectRequest request, CancellationToken ct)
    {
        // GN-09-R1
        if (!await users.ExistsAsync(currentUserId, ct))
            return ServiceResult<ProjectResponse>.Unauthorized();

        var errors = new FieldErrors();
        var name = errors.Text(nameof(request.Name), request.Name, min: 2, max: 100);
        var description = errors.OptionalText(nameof(request.Description), request.Description, max: 1000);
        if (errors.HasErrors)
            return ServiceResult<ProjectResponse>.Validation(errors.ToDictionary());

        // EN-02-R1
        if (await projects.NameExistsAsync(name, null, ct))
            return ServiceResult<ProjectResponse>.Conflict(ErrorMessages.ProjectNameTaken);

        var project = new Project { Name = name, Description = description, CreatedAt = time.GetUtcNow().UtcDateTime };
        // GN-12: a parallel request with the same name fails on the unique index
        if (await projects.AddAsync(project, ct) == WriteResult.Duplicate)
            return ServiceResult<ProjectResponse>.Conflict(ErrorMessages.ProjectNameTaken);

        return ServiceResult<ProjectResponse>.Success(project.ToResponse(0));
    }

    // FN-AD-07
    public async Task<ServiceResult<ProjectResponse>> UpdateAsync(int currentUserId, int id, ProjectRequest request, CancellationToken ct)
    {
        if (!await users.ExistsAsync(currentUserId, ct))
            return ServiceResult<ProjectResponse>.Unauthorized();

        var project = await projects.FindAsync(id, ct);
        if (project is null)
            return ServiceResult<ProjectResponse>.NotFound();

        var errors = new FieldErrors();
        var name = errors.Text(nameof(request.Name), request.Name, min: 2, max: 100);
        var description = errors.OptionalText(nameof(request.Description), request.Description, max: 1000);
        if (errors.HasErrors)
            return ServiceResult<ProjectResponse>.Validation(errors.ToDictionary());

        // The same name for the same project is allowed
        if (await projects.NameExistsAsync(name, id, ct))
            return ServiceResult<ProjectResponse>.Conflict(ErrorMessages.ProjectNameTaken);

        project.Name = name;
        project.Description = description;
        if (await projects.UpdateAsync(project, ct) == WriteResult.Duplicate)
            return ServiceResult<ProjectResponse>.Conflict(ErrorMessages.ProjectNameTaken);

        var taskCount = await projects.CountTasksAsync(id, ct);
        return ServiceResult<ProjectResponse>.Success(project.ToResponse(taskCount));
    }

    // FN-AD-08
    public async Task<ServiceResult> DeleteAsync(int currentUserId, int id, CancellationToken ct)
    {
        if (!await users.ExistsAsync(currentUserId, ct))
            return ServiceResult.Unauthorized();

        var project = await projects.FindAsync(id, ct);
        if (project is null)
            return ServiceResult.NotFound();

        // RE-01-R1
        if (await projects.HasTasksAsync(id, ct))
            return ServiceResult.Conflict(ErrorMessages.ProjectHasTasks);

        await projects.DeleteAsync(project, ct);
        return ServiceResult.Success();
    }
}
