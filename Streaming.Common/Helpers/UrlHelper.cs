using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

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
