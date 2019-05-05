using Streaming.Common.Exceptions;
using Streaming.Common.Helpers;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Common.Extensions
{
    public static class BashExtensions
    {
        public enum DefaultOutput
        {
            StandardOutput,
            ErrorOutput
        }

        public static Process StartBashExecution(this string command)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = PlatformHelper.CommandlineToolname;
            psi.Arguments = $"-c \"{command}\"";
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

        /// <summary>
        /// Execute command and returns command output
        /// </summary>
        /// <param name="command">command string</param>
        /// <param name="defaultOutput">Default output for read</param>
        /// <param name="commandLineOutputCallback">Callback that is called for every output line readed</param>
        /// <returns></returns>
        public static async Task<string> ExecuteBashAsync(this string command, DefaultOutput defaultOutput = DefaultOutput.StandardOutput, Action<string> commandLineOutputCallback = null)
        {
            string errorOutput = "";
            var strBuilder = new StringBuilder();
            try
            {
                using (var process = command.StartBashExecution())
                {
                    var defaultStream = defaultOutput == DefaultOutput.StandardOutput ? process.StandardOutput : process.StandardError;
                    while(!defaultStream.EndOfStream)
                    {
                        var line = await defaultStream.ReadLineAsync();
                        strBuilder.AppendLine(line);
                        commandLineOutputCallback?.Invoke(line);
                    }
                    if (defaultOutput == DefaultOutput.StandardOutput && !process.StandardError.EndOfStream)
                    {
                        errorOutput += await process.StandardError.ReadToEndAsync();
                    }
                    process.WaitForExit();
                    if (process.ExitCode != 0)
                        throw new CommandException(command, defaultOutput == DefaultOutput.StandardOutput ? errorOutput : strBuilder.ToString());
                    return strBuilder.ToString();
                }
            }
            catch(Exception ex)
            {
                throw new CommandException(command, errorOutput, ex);
            }
        }
    }
}
