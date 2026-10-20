using System.Diagnostics;
using System.Text.RegularExpressions;

namespace TaskManager.Api.Middleware;

// MW-02: one id per request (or the client's), in every log line, in the response header, and one Information line per request
public partial class CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
{
    public const string Header = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        var id = context.Request.Headers[Header].ToString();
        // Only a short, plain id from the client, so it cannot inject text into the log
        if (!SafeId().IsMatch(id))
            id = Guid.NewGuid().ToString("N");

        // GN-03: ProblemDetails reads correlationId from TraceIdentifier
        context.TraceIdentifier = id;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[Header] = id;
            return Task.CompletedTask;
        });

        using var scope = logger.BeginScope("CorrelationId:{CorrelationId}", id);
        var started = Stopwatch.GetTimestamp();
        var failed = false;
        try
        {
            await next(context);
        }
        catch
        {
            // MW-01 (outside) turns it into a 500
            failed = true;
            throw;
        }
        finally
        {
            // GN-06: method, path, status, duration, CorrelationId; never the body (GN-06-R1)
            logger.LogInformation("{Method} {Path} responded {StatusCode} in {ElapsedMs} ms [{CorrelationId}]",
                context.Request.Method, context.Request.Path, failed ? StatusCodes.Status500InternalServerError : context.Response.StatusCode,
                (long)Stopwatch.GetElapsedTime(started).TotalMilliseconds, id);
        }
    }

    [GeneratedRegex("^[A-Za-z0-9._-]{1,64}$")]
    private static partial Regex SafeId();
}
