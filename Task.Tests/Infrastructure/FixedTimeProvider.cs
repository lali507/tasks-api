namespace TaskManager.Tests.Infrastructure;

// A clock that always returns the same moment, so CreatedAt and CompletedAt can be asserted
public class FixedTimeProvider(DateTimeOffset now) : TimeProvider
{
    public static readonly DateTimeOffset Default = new(2026, 5, 1, 10, 0, 0, TimeSpan.Zero);

    public FixedTimeProvider() : this(Default) { }

    public override DateTimeOffset GetUtcNow() => now;
}
