using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Results;

namespace TaskManager.Api.Middleware;

// MW-01: wraps the whole pipeline; an unhandled exception -> Error log and 500 ProblemDetails without internal details (GN-03-R1)
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context, IProblemDetailsService problemDetails)
    {
        try
        {
            await next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // The client went away; nothing to answer
        }
        catch (Exception ex)
        {
            // GN-06: CorrelationId and the exception type. MW-02 runs inside this one, so its scope is closed here.
            using (logger.BeginScope("CorrelationId:{CorrelationId}", context.TraceIdentifier))
                logger.LogError(ex, "Unhandled {ExceptionType}", ex.GetType().Name);

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            // Written through the ProblemDetails service, so correlationId is added (GN-03)
            await problemDetails.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                ProblemDetails = new ProblemDetails { Status = StatusCodes.Status500InternalServerError, Title = ErrorMessages.ServerError },
            });
        }
    }
}
