using System;

namespace Streaming.Common.Exceptions
{
    public class CommandException : Exception
    {
        public CommandException(string command, string output) : this(command, output, null)
        {
        }

        public CommandException(string command, string output, Exception innerException)  : base($"Command: \"{command}\", Output: \"{output}\"", innerException)
        {
        }
    }
}
