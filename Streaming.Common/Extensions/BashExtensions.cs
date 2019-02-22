using Streaming.Common.Exceptions;
using Streaming.Common.Helpers;
using System;
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
            string errorOutput = "";
            try
            {
                using (var process = Command.StartBashExecution())
                {
                    errorOutput = await process.StandardError.ReadToEndAsync();

                    string output = await process.StandardOutput.ReadToEndAsync();
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                        throw new CommandException(Command, errorOutput);
                    return output;
                }
            }
            catch(Exception ex)
            {
                throw new CommandException(Command, errorOutput, ex);
            }
        }
    }
}
