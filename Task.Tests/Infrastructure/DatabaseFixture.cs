namespace TaskManager.Tests.Infrastructure;

// Tests against PostgreSQL share one fresh database and run one class at a time, so lists and counts stay predictable
[CollectionDefinition(Name)]
public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
    public const string Name = "Database";
}

public class DatabaseFixture : IAsyncLifetime
{
    public ApiFactory Api { get; } = new();

    public Task InitializeAsync() => TestDatabase.EnsureFreshAsync();

    public async Task DisposeAsync() => await Api.DisposeAsync();
}
