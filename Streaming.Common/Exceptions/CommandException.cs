using System;

namespace Streaming.Common.Exceptions
{
    public class CommandException : Exception
    {
        public CommandException(string command, int exitCode, string output) : this(command, exitCode, output, null)
        {
        }

        public CommandException(string command, int exitCode, string output, Exception innerException)  
            : base($"Command: \"{command}\", ExitCode: {exitCode}, Output: \"{output}\"", innerException)
        {
        }
    }
}
