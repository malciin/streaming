using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Streaming.Common.Exceptions;

namespace Streaming.Common.EmbedProcesses
{
    public class EmbeddedProcess : IDisposable
    {
        private string ProgramName { get; set; }
        private string Args { get; set; }
        private Process Process { get; set; }

        private readonly TaskCompletionSource<string> tsc;
        private readonly StringBuilder standardOutputBuilder;
        private readonly StringBuilder standardErrorBuilder;
        private bool processStarted = false;
        
        public bool HasExited => Process.HasExited;
        public int ExitCode => Process.ExitCode;

        private EmbeddedProcess()
        {
            tsc = new TaskCompletionSource<string>();
            standardErrorBuilder = new StringBuilder();
            standardOutputBuilder = new StringBuilder();
        }
        
        public void AddWatcher(IEmbeddedProcessWatcher watcher)
        {
            Process.ErrorDataReceived += watcher.ErrorDataReceived;
            Process.OutputDataReceived += watcher.OutputDataReceived;
        }
        
        public async Task<string> HandleAsync()
        {
            using (Process)
            {
                if (!processStarted)
                {
                    Start();
                }
                Process.BeginOutputReadLine();
                Process.BeginErrorReadLine();

                return await tsc.Task;
            }
        }

        public void Start()
        {
            Process.OutputDataReceived += (sender, args) => standardOutputBuilder.AppendLine(args.Data);
            Process.ErrorDataReceived += (sender, args) => standardErrorBuilder.AppendLine(args.Data);
            Process.Exited += (sender, args) =>
            {
                if (Process.ExitCode != 0)
                {
                    tsc.TrySetException(
                        new CommandException($"{ProgramName} {Args}", Process.ExitCode, standardErrorBuilder.ToString()));
                }

                tsc.TrySetResult(standardOutputBuilder.ToString());
            };
            processStarted = true;
            
            Process.Start();
        }

        public void Dispose()
        {
            Process?.Dispose();
        }

        public static EmbeddedProcess Create(string programName, string args)
        {
            var psi = new ProcessStartInfo();
            psi.FileName = programName;
            psi.Arguments = args;
            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;

            var process = new Process
            {
                StartInfo = psi,
                EnableRaisingEvents = true
            };

            return new EmbeddedProcess
            {
                Process = process,
                ProgramName = programName,
                Args = args
            };
        }
    }
}