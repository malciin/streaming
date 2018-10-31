using System;
using Microsoft.AspNetCore.Http;

namespace Streaming.Api.Monitor
{
    public interface ICustomLogger
    {
        void Log(HttpContext Context, Exception Exc);
    }
}
