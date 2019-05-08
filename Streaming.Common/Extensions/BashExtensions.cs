using Streaming.Common.Helpers;
using System.Threading.Tasks;
using Streaming.Common.EmbedProcesses;

namespace Streaming.Common.Extensions
{
    public static class BashExtensions
    {
        /// <summary>
        /// Execute command and returns command output.
        /// Throws CommandException if command return different ExitCode than 0
        /// </summary>
        public static Task<string> ExecuteBashAsync(this string command)
        {
            return EmbeddedProcess.Create(PlatformHelper.CommandlineToolname, $"-c \"{command}\"").GetResultAsync();
        }
    }
}
