using Streaming.Common.Helpers;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Streaming.Common.Extensions
{
    public static class BashExtensions
    {
        private static Process StartCommandProcess(string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "bash";
            psi.Arguments = $"-c \"{command}\"";
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            Process proc = new Process
            {
                StartInfo = psi
            };

            proc.Start();
            return proc;
        }

        public static string ExecuteBash(this string Command)
        {
            var proc = StartCommandProcess(Command);

            string error = proc.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error))
                return "error: " + error;

            string output = proc.StandardOutput.ReadToEnd();
            proc.WaitForExit();
            return output;
        }

        public static async Task<string> ExecuteBashAsync(this string Command)
        {
            var proc = StartCommandProcess(Command);

            string error = await proc.StandardError.ReadToEndAsync();

            if (!string.IsNullOrEmpty(error))
                return "error: " + error;

            string output = await proc.StandardOutput.ReadToEndAsync();
            proc.WaitForExit();
            return output;
        }
    }
}
