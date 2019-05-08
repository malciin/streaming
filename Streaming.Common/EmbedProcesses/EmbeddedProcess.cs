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

        private readonly StringBuilder standardOutputBuilder;
        private readonly StringBuilder standardErrorBuilder;
        private bool processStarted = false;
        
        private EmbeddedProcess()
        {
            standardErrorBuilder = new StringBuilder();
            standardOutputBuilder = new StringBuilder();
        }
        
        public void AddWatcher(IEmbeddedProcessWatcher watcher)
        {
            Process.ErrorDataReceived += watcher.ErrorDataReceived;
            Process.OutputDataReceived += watcher.OutputDataReceived;
        }
        
        /// <summary>
        /// Get the standard output from executed command if the returned code == 0
        /// Otherwise throw CommandException
        /// </summary>
        /// <returns>Output from executed command</returns>
        public async Task<string> GetResultAsync()
        {
            using (Process)
            {
                var tsc = new TaskCompletionSource<string>();
                if (!processStarted)
                {
                    StartProcess((sender, args) =>
                    {
                        if (Process.ExitCode != 0)
                        {
                            tsc.TrySetException(
                                new CommandException($"{ProgramName} {Args}", Process.ExitCode, standardErrorBuilder.ToString()));
                        }

                        tsc.TrySetResult(standardOutputBuilder.ToString());
                    });
                }
                Process.BeginOutputReadLine();
                Process.BeginErrorReadLine();

                return await tsc.Task;
            }
        }
        
        /// <summary>
        /// Provide more details about finished process like ResponseCode and both error and standard output
        /// </summary>
        public async Task<ExecutionResult> GetExecutionResultAsync()
        {
            using (Process)
            {
                var tsc = new TaskCompletionSource<ExecutionResult>();
                if (!processStarted)
                {
                    StartProcess((sender, args) =>
                    {
                        tsc.TrySetResult(new ExecutionResult
                        {
                            ReturnCode = Process.ExitCode,
                            ErrorOutput = standardErrorBuilder.ToString(),
                            StandardOutput = standardOutputBuilder.ToString()
                        });
                    });
                }
                Process.BeginOutputReadLine();
                Process.BeginErrorReadLine();

                return await tsc.Task;
            }
        }
    
        private void StartProcess(EventHandler exitedEventStrategy)
        {
            Process.OutputDataReceived += (sender, args) => standardOutputBuilder.AppendLine(args.Data);
            Process.ErrorDataReceived += (sender, args) => standardErrorBuilder.AppendLine(args.Data);
            Process.Exited += exitedEventStrategy;
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

        public class ExecutionResult
        {
            public int ReturnCode { get; set; }
            public string StandardOutput { get; set; }
            public string ErrorOutput { get; set; }
        }
    }
}