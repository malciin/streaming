using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;

namespace Streaming.Api.FileLogger
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private readonly ConcurrentDictionary<string, Lazy<StreamWriter>> streams;
        private readonly string logsDirectory;

        public FileLoggerProvider(string logsDirectory)
        {
            this.streams = new ConcurrentDictionary<string, Lazy<StreamWriter>>();
            this.logsDirectory = logsDirectory;
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (!streams.ContainsKey(categoryName))
            {
                streams.TryAdd(categoryName, new Lazy<StreamWriter>(() => GetStreamWriter(categoryName),
                    LazyThreadSafetyMode.ExecutionAndPublication));
            }
            return new FileLogger(streams[categoryName]);
        }

        private StreamWriter GetStreamWriter(string categoryName)
        {
            var now = DateTime.UtcNow;
            var outputPath = Path.Combine(logsDirectory, $"{now.Year}-{now.Month}-{now.Day}");
            Directory.CreateDirectory(outputPath);
            outputPath = Path.Combine(outputPath, $"{categoryName}.log");
            var outputStream = new StreamWriter(
                stream: Stream.Synchronized(File.Open(outputPath, FileMode.Append, FileAccess.Write, FileShare.Read)),
                encoding: Encoding.UTF8) { AutoFlush = true };
            return outputStream;
        }

        public void Dispose()
        {
            foreach(var (_, lazyStreamWriter) in streams)
            {
                if (lazyStreamWriter.IsValueCreated)
                    lazyStreamWriter.Value.Dispose();
            }
        }
    }
}
