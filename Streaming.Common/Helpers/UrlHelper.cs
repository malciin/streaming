using Microsoft.AspNetCore.Http;

namespace Streaming.Common.Helpers
{
    public static class UrlHelper
    {
        public static string GetHostUrl(HttpContext httpContext)
        {
            return httpContext.Request.Scheme + "://" + httpContext.Request.Host;
        }
    }
}
