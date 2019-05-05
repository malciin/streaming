using Microsoft.AspNetCore.Http;
using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Streaming.Application.Exceptions;
using Streaming.Common.Exceptions;

namespace Streaming.Api.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;


        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (CommandException ex)
            {
                handleException(context, ex, "Command not finished successfully", generateRequestDetails: true);
            }
            catch (NotVideoFileException ex)
            {
                handleException(context, ex, "The uploaded file is not a video file", generateRequestDetails: true);
            }
        }

        private void handleException<T>(HttpContext context, T exception, string logMessage, bool generateRequestDetails = false) where T : Exception
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(logMessage);

            stringBuilder.Append(GenerateRequestDetails(context));

            logger.LogError(exception, stringBuilder.ToString());
        }

        private static string GenerateRequestDetails(HttpContext context)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Request info:");

            if (context.User.Identity?.IsAuthenticated == true)
            {
                stringBuilder.AppendLine($"\t User: {context.User.Identity.Name}");
            }
            else
            {
                stringBuilder.AppendLine($"\t User: Anonymous");
            }

            stringBuilder.AppendLine($"\t Path: {context.Request.Path.ToString()}");
            stringBuilder.AppendLine($"\t Method: {context.Request.Method.ToString()}");
            stringBuilder.AppendLine($"\t Host: {context.Request.Host.ToString()}");
            stringBuilder.AppendLine($"\t Headers: ");
            foreach (var (headerKey, value) in context.Request.Headers)
            {
                stringBuilder.AppendLine($"\t\t{headerKey}: {value}");
            }

            return stringBuilder.ToString();
        }
    }
}
