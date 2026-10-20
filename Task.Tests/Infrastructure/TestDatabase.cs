using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskManager.Data;

namespace TaskManager.Tests.Infrastructure;

// GN-05-R3: the test database (TaskDb_Test) is recreated once per test run, with the same seed as development
public static class TestDatabase
{
    private static readonly SemaphoreSlim Gate = new(1, 1);
    private static bool _ready;

    // User Secrets of the API in development; ConnectionStrings__TestConnection anywhere else
    public static string ConnectionString { get; } =
        new ConfigurationBuilder()
            .AddUserSecrets<Program>(optional: true)
            .AddEnvironmentVariables()
            .Build()
            .GetConnectionString("TestConnection")
        ?? throw new InvalidOperationException("ConnectionStrings:TestConnection is not set (User Secrets of Task.api or an environment variable).");

    public static TaskDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<TaskDbContext>().UseNpgsql(ConnectionString).Options);

    public static async Task EnsureFreshAsync()
    {
        await Gate.WaitAsync();
        try
        {
            if (_ready)
                return;
            await using var db = CreateContext();
            await db.Database.EnsureDeletedAsync();
            // The migrations include the seed data (GN-05)
            await db.Database.MigrateAsync();
            _ready = true;
        }
        finally
        {
            Gate.Release();
        }
    }
}
