using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Extensions;
using TaskManager.Core.DTOs.Queries;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Interfaces.Services;

namespace TaskManager.Api.Controllers;

// AP-01 to AP-03: thin controller over IUserService
[Route("api/users")]
public class UsersController(IUserService users) : ApiControllerBase
{
    // FN-AD-15
    [HttpGet]
    public async Task<ActionResult<PagedResponse<UserResponse>>> GetPaged([FromQuery] PagingQuery query, CancellationToken ct) =>
        OkOrProblem(await users.GetPagedAsync(query, ct));

    // FN-AD-16: no token in the response
    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create([FromBody] CreateUserRequest request, CancellationToken ct) =>
        CreatedOrProblem(await users.CreateAsync(User.GetUserId(), request, ct), u => $"/api/users/{u.Id}");

    // FN-AD-17
    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserResponse>> Update([FromRoute] int id, [FromBody] UpdateUserRequest request, CancellationToken ct) =>
        OkOrProblem(await users.UpdateAsync(User.GetUserId(), id, request, ct));

    // FN-AD-18
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken ct) =>
        NoContentOrProblem(await users.DeleteAsync(User.GetUserId(), id, ct));
}
