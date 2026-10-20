using Microsoft.AspNetCore.Mvc;
using TaskManager.Api.Extensions;
using TaskManager.Core.DTOs.Requests;
using TaskManager.Core.DTOs.Responses;
using TaskManager.Core.Interfaces.Services;

namespace TaskManager.Api.Controllers;

// AP-01 to AP-03: thin controller over ILabelService
[Route("api/labels")]
public class LabelsController(ILabelService labels) : ApiControllerBase
{
    // FN-MB-05, FN-AD-05: not paged (FN-MB-05-R1)
    [HttpGet]
    public async Task<ActionResult<List<LabelResponse>>> GetAll(CancellationToken ct) =>
        OkOrProblem(await labels.GetAllAsync(ct));

    // FN-AD-12
    [HttpPost]
    public async Task<ActionResult<LabelResponse>> Create([FromBody] LabelRequest request, CancellationToken ct) =>
        CreatedOrProblem(await labels.CreateAsync(User.GetUserId(), request, ct), l => $"/api/labels/{l.Id}");

    // FN-AD-13
    [HttpPut("{id:int}")]
    public async Task<ActionResult<LabelResponse>> Update([FromRoute] int id, [FromBody] LabelRequest request, CancellationToken ct) =>
        OkOrProblem(await labels.UpdateAsync(User.GetUserId(), id, request, ct));

    // FN-AD-14
    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken ct) =>
        NoContentOrProblem(await labels.DeleteAsync(User.GetUserId(), id, ct));
}
