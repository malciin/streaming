using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text;

namespace Streaming.Api.Monitor
{
    public class CustomLogger : ICustomLogger
    {
        static object locker = new object();
        static string logFolder = "logs";

        static CustomLogger()
        {
            System.IO.Directory.CreateDirectory(logFolder);
        }

        private void MapExceptionDetails(StringBuilder builder, Exception exception)
        {
            builder.AppendLine(exception.Message);
            builder.AppendLine(exception.StackTrace);
            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                builder.AppendLine("-");
                builder.AppendLine(exception.Message);
                builder.AppendLine(exception.StackTrace);
            }
        }

        private void MapRequestDetails(StringBuilder builder, HttpContext httpContext)
        {

            builder.AppendLine($"Request info:");
            if (httpContext?.User?.Identity != null && httpContext.User.Identity.IsAuthenticated)
            {
                builder.AppendLine($"\t User: {httpContext.User.Identity.Name}");
            }
            else
            {
                builder.AppendLine($"\t User: Anonymous");
            }

            builder.AppendLine($"\t Path: {httpContext.Request.Path.ToString()}");
            builder.AppendLine($"\t Method: {httpContext.Request.Method.ToString()}");
            builder.AppendLine($"\t Host: {httpContext.Request.Host.ToString()}");
            builder.AppendLine($"\t Headers: ");
            MapRequestHeaders(builder, httpContext.Request.Headers);
            builder.AppendLine($"\t Body:");
        }

        private void MapRequestBody(StringBuilder builder, Stream body)
        {
            if (body.Length > 1024)
            {
                builder.AppendLine($"Body exceeded 1kb log-limit");
                return;
            }
            var memoryStream = new MemoryStream();
            body.CopyTo(memoryStream);
            builder.AppendLine(Encoding.UTF8.GetString(memoryStream.ToArray()));
        }

        private void MapRequestHeaders(StringBuilder builder, IHeaderDictionary headers)
        {
            foreach(var header in headers)
            {
                builder.AppendLine($"\t\t{header.Key}: {header.Value}");
            }
        }

        public void Log(HttpContext Context, Exception exception)
        {
            var currentDate = DateTime.UtcNow;
            var fileName = $"{logFolder}/{currentDate.Year}-{currentDate.Month}-{currentDate.Day}.txt";

            StringBuilder logStringBuilder = new StringBuilder();
            logStringBuilder.AppendLine("---");

            logStringBuilder.AppendLine($"{currentDate}: ");

            MapRequestDetails(logStringBuilder, Context);
            MapExceptionDetails(logStringBuilder, exception);
            

            logStringBuilder.AppendLine("---");
            logStringBuilder.AppendLine();
            
            lock (locker) {
                File.AppendAllText(fileName, logStringBuilder.ToString(), Encoding.UTF8);
            }
        }

       
    }
}
