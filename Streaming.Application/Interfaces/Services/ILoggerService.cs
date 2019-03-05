using System;

namespace Streaming.Application.Interfaces.Services
{
    public interface ILoggerService
    {
        /// <summary>
        /// Log certain message to log files
        /// </summary>
        /// <param name="Caller">Calling object, or the object that raised exception. For most cases send 'this'</param>
        /// <param name="Message">Message to log, for ex. exceptions etc</param>
        void Log(object Caller, string Message);

        /// <summary>
        /// Log certain exception to log files
        /// </summary>
        /// <param name="Caller">Calling object, or the object that raised exception. For most cases send 'this'</param>
        /// <param name="Exception">Exception to log</param>
        void Log(object Caller, Exception Exception);
    }
}
