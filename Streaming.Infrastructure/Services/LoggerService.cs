using Streaming.Application.Interfaces.Services;
using Streaming.Application.Interfaces.Settings;
using System;
using System.IO;
using System.Text;

namespace Streaming.Infrastructure.Services
{
    public class LoggerService : ILoggerService
    {
        private readonly DirectoryInfo outputDirectory;
        private readonly object locker = new object();

        public LoggerService(ILogsDirectorySettings logsDirectory)
        {
            outputDirectory = Directory.CreateDirectory(logsDirectory.LogsDirectory);
        }

        private string getCurrentLogFileName()
        {
            var now = DateTime.UtcNow;
            return String.Format($"{outputDirectory.FullName}{{0}}{now.Day}-{now.Month}-{now.Year}.txt",
                Path.DirectorySeparatorChar);
        }

        private string createLogMessage(object caller, string message)
        {
            var strBuilder = new StringBuilder();
            var callerType = caller.GetType().FullName;

            strBuilder.Append($"[{DateTime.UtcNow.ToString("HH-mm-ss")} {callerType}] {message}");

            return strBuilder.ToString();
        }

        public void Log(object Caller, string Message)
        {
            lock (locker)
            {
                File.AppendAllText(getCurrentLogFileName(), createLogMessage(Caller, Message) + Environment.NewLine, Encoding.UTF8);
            }
        }

        public void Log(object Caller, Exception Exception)
        {
            var message = String.Empty;
            while(Exception != null)
            {
                message += Exception.Message;
                Exception = Exception.InnerException;
            }
            Log(Caller, message);
        }
    }
}
