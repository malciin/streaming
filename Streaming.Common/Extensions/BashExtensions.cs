using System.Diagnostics;

namespace Streaming.Common.Extensions
{
    public static class BashExtensions
    {
        public static string ExecuteBash(this string Command)
        {
            ProcessStartInfo psi = new ProcessStartInfo();
            psi.FileName = "bash";
            psi.Arguments = $"-c \"{Command}\"";
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            Process proc = new Process
            {
                StartInfo = psi
            };


            proc.Start();

            string error = proc.StandardError.ReadToEnd();

            if (!string.IsNullOrEmpty(error))
                return "error: " + error;

            string output = proc.StandardOutput.ReadToEnd();

            proc.WaitForExit();

            return output.Replace("\n", "").Replace("\r", "");
        }
    }
}
