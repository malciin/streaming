using Streaming.Common.Helpers;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Streaming.Common.Extensions
{
    public static class BashExtensions
    {
        public static Process StartBashExecution(this string Command)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = PlatformHelper.CommandlineToolname;
            psi.Arguments = $"-c \"{Command}\"";
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            Process process = new Process
            {
                StartInfo = psi
            };

            process.Start();
            return process;
        }

        public static async Task<string> ExecuteBashAsync(this string Command)
        {
            using (var process = Command.StartBashExecution())
            {
                string error = await process.StandardError.ReadToEndAsync();

                if (!string.IsNullOrEmpty(error))
                    return "error: " + error;

                string output = await process.StandardOutput.ReadToEndAsync();
                process.WaitForExit();
                return output;
            }
        }
    }
}
