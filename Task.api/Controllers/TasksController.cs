using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Extensions;
using TaskManager.Core.DTOs.Queries;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Interfaces.Services;

namespace TaskManager.Api.Controllers;

// AP-01 to AP-03: thin controller over ITaskService
[Route("api/tasks")]
public class TasksController(ITaskService tasks) : ApiControllerBase
{
    // FN-MB-03, FN-AD-03: status, projectId, mine, page, pageSize (GN-02)
    [HttpGet]
    public async Task<ActionResult<PagedResponse<TaskResponse>>> GetPaged([FromQuery] TaskQuery query, CancellationToken ct) =>
        OkOrProblem(await tasks.GetPagedAsync(User.GetUserId(), query, ct));

    // FN-MB-04, FN-AD-04
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponse>> GetById([FromRoute] int id, CancellationToken ct) =>
        OkOrProblem(await tasks.GetByIdAsync(id, ct));

    // FN-AD-09
    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create([FromBody] TaskRequest request, CancellationToken ct) =>
        CreatedOrProblem(await tasks.CreateAsync(User.GetUserId(), request, ct), t => $"/api/tasks/{t.Id}");

    // FN-AD-10
    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskResponse>> Update([FromRoute] int id, [FromBody] TaskRequest request, CancellationToken ct) =>
        OkOrProblem(await tasks.UpdateAsync(User.GetUserId(), id, request, ct));

    // FN-AD-11
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken ct) =>
        NoContentOrProblem(await tasks.DeleteAsync(User.GetUserId(), id, ct));

    // FN-MB-06: no body; the user comes from the token
    [HttpPut("{id:int}/assignee")]
    public async Task<ActionResult<TaskResponse>> Take([FromRoute] int id, CancellationToken ct) =>
        OkOrProblem(await tasks.TakeAsync(User.GetUserId(), id, ct));

    // FN-MB-07: no body (FN-MB-07-R1)
    [HttpPut("{id:int}/completion")]
    public async Task<ActionResult<TaskResponse>> Complete([FromRoute] int id, CancellationToken ct) =>
        OkOrProblem(await tasks.CompleteAsync(User.GetUserId(), id, ct));
}
