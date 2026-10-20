using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Auth;
using TaskManager.Api.Extensions;
using TaskManager.Core.DTOs.Queries;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Interfaces.Services;

namespace TaskManager.Api.Controllers;

// AP-01 to AP-03: thin controller over IProjectService
[Route("api/projects")]
public class ProjectsController(IProjectService projects) : ApiControllerBase
{
    // FN-MB-01, FN-AD-01
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProjectResponse>>> GetPaged([FromQuery] PagingQuery query, CancellationToken ct) =>
        OkOrProblem(await projects.GetPagedAsync(query, ct));

    // FN-MB-02, FN-AD-02
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ProjectDetailsResponse>> GetById([FromRoute] int id, CancellationToken ct) =>
        OkOrProblem(await projects.GetByIdAsync(id, ct));

    // FN-AD-06
    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<ActionResult<ProjectResponse>> Create([FromBody] ProjectRequest request, CancellationToken ct) =>
        CreatedOrProblem(await projects.CreateAsync(User.GetUserId(), request, ct), p => $"/api/projects/{p.Id}");

    // FN-AD-07
    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ProjectResponse>> Update([FromRoute] int id, [FromBody] ProjectRequest request, CancellationToken ct) =>
        OkOrProblem(await projects.UpdateAsync(User.GetUserId(), id, request, ct));

    // FN-AD-08
    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken ct) =>
        NoContentOrProblem(await projects.DeleteAsync(User.GetUserId(), id, ct));
}
