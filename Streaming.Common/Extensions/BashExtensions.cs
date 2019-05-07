using Streaming.Common.Helpers;
using System.Threading.Tasks;
using Streaming.Common.EmbedProcesses;

namespace Streaming.Common.Extensions
{
    public static class BashExtensions
    {
        private static EmbeddedProcess StartBashExecution(this string command)
        {
            var process = EmbeddedProcess.Create(PlatformHelper.CommandlineToolname, $"-c \"{command}\"");
            process.Start();
            return process;
        }

        /// <summary>
        /// Execute command and returns command output.
        /// Throws CommandException if command return different ExitCode than 0
        /// </summary>
        public static Task<string> ExecuteBashAsync(this string command)
        {
            return StartBashExecution(command).HandleAsync();
        }
    }
}
