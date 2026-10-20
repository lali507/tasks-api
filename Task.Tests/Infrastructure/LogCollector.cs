using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace TaskManager.Tests.Infrastructure;

// Keeps every log line the in-memory API writes, so tests can check GN-06 (Warning on a conflict, no secrets)
public class LogCollector : ILoggerProvider
{
    public record Entry(string Category, LogLevel Level, string Message, string? Exception);

    private readonly ConcurrentQueue<Entry> _entries = new();

    public IReadOnlyList<Entry> Entries => [.. _entries];

    public void Clear() => _entries.Clear();

    public ILogger CreateLogger(string categoryName) => new Logger(categoryName, _entries);

    public void Dispose() { }

    private sealed class Logger(string category, ConcurrentQueue<Entry> entries) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
            entries.Enqueue(new Entry(category, logLevel, formatter(state, exception), exception?.ToString()));
    }
}
