using System;

namespace Streaming.Application.Exceptions
{
    public class NotVideoFileException : Exception
    {
        public NotVideoFileException(string path, Exception inner) : base($"File {path} is not a video type", inner)
        {
        }

        public NotVideoFileException(string path) : this(path, null)
        {
        }
    }
}
