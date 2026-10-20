using System.Net;
using System.Text.Json;

namespace TaskManager.Tests.Infrastructure;

// GN-03: the parts of a ProblemDetails response the tests check
public record Problem(int Status, string Title, string? CorrelationId, Dictionary<string, string[]>? Errors)
{
    public static async Task<Problem> ReadAsync(HttpResponseMessage response)
    {
        var problem = JsonSerializer.Deserialize<Problem>(await response.Content.ReadAsStringAsync(), Json.Options)!;
        Assert.Equal((int)response.StatusCode, problem.Status);
        Assert.False(string.IsNullOrEmpty(problem.CorrelationId));
        return problem;
    }

    // Asserts the status and the GN-07 title in one go
    public static async Task<Problem> AssertAsync(HttpResponseMessage response, HttpStatusCode status, string title)
    {
        Assert.Equal(status, response.StatusCode);
        var problem = await ReadAsync(response);
        Assert.Equal(title, problem.Title);
        return problem;
    }
}
