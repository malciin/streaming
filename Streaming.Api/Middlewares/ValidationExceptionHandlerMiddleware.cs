using FluentValidation;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Api.Middlewares
{
    public class ValidationExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;

        public ValidationExceptionHandlerMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(ex.Message, Encoding.UTF8);
                return;
            }
        }
    }
}
