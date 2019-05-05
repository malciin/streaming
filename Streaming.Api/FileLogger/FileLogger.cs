using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace Streaming.Api.FileLogger
{
    public class FileLogger : ILogger
    {
        private class LoggerScope : IDisposable
        {
            public void Dispose()
            {
            }
        }

        private StreamWriter logOutput => lazyLogOutput.Value;

        private readonly Lazy<StreamWriter> lazyLogOutput;

        public FileLogger(Lazy<StreamWriter> lazyLogOutput)
        {
            this.lazyLogOutput = lazyLogOutput;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new LoggerScope();
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var now = DateTime.UtcNow;
            logOutput.WriteLine($"[{Enum.GetName(typeof(LogLevel), logLevel)}: {DateTime.UtcNow:yyyy'-'MM'-'dd' 'HH':'mm':'ss}] {formatter(state, exception)}");
        }
    }
}
