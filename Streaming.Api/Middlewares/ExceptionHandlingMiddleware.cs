using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Streaming.Api.Monitor;
using System;
using System.Threading.Tasks;

namespace Streaming.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ICustomLogger customLogger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ICustomLogger customLogger)
        {
            this.next = next;
            this.customLogger = customLogger;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            customLogger.Log(context, exception);
            var result = JsonConvert.SerializeObject(new {
                Error = exception.Message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 500;
            return context.Response.WriteAsync(result);
        }
    }
}
