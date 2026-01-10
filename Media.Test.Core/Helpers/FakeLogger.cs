using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace Media.Test.Core.Helpers
{
    public class FakeLogger<T> : ILogger<T>
    {
        public ConcurrentQueue<string> Messages = new();

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;

        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, 
			Exception? exception, Func<TState, Exception?, string> formatter) =>
            this.Messages.Enqueue(formatter(state, exception));
    }

    public sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();
        public void Dispose() { }
    }
}
