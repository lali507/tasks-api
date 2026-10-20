using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Results;

namespace TaskManager.Api.Controllers;

// AP-01: turns the Service result (CO-05) into an HTTP code; failures become ProblemDetails with the GN-07 title (GN-03)
[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected ActionResult OkOrProblem<T>(ServiceResult<T> result) =>
        result.IsSuccess ? Ok(result.Value) : ToProblem(result);

    // AP-03: 201 always with a Location header
    protected ActionResult CreatedOrProblem<T>(ServiceResult<T> result, Func<T, string> location) =>
        result.IsSuccess ? Created(location(result.Value!), result.Value) : ToProblem(result);

    protected ActionResult NoContentOrProblem(ServiceResult result) =>
        result.IsSuccess ? NoContent() : ToProblem(result);

    private ActionResult ToProblem(ServiceResult result)
    {
        if (result.Status == ResultStatus.Validation)
        {
            var errors = result.Errors!.ToDictionary(e => e.Key, e => e.Value);
            return ValidationProblem(new ValidationProblemDetails(errors) { Status = StatusCodes.Status400BadRequest, Title = result.Title });
        }

        var status = result.Status switch
        {
            ResultStatus.Unauthorized => StatusCodes.Status401Unauthorized,
            ResultStatus.Forbidden => StatusCodes.Status403Forbidden,
            ResultStatus.NotFound => StatusCodes.Status404NotFound,
            ResultStatus.Conflict => StatusCodes.Status409Conflict,
            _ => throw new InvalidOperationException($"Unexpected result status {result.Status}"),
        };
        return Problem(statusCode: status, title: result.Title);
    }
}
