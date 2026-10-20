using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            var modelState = new ModelStateDictionary();
            foreach (var (field, messages) in result.Errors!)
                foreach (var message in messages)
                    modelState.AddModelError(field, message);
            // Through the factory, so correlationId is added like on every other error (GN-03)
            return ValidationProblem(ProblemDetailsFactory.CreateValidationProblemDetails(
                HttpContext, modelState, StatusCodes.Status400BadRequest, result.Title));
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
