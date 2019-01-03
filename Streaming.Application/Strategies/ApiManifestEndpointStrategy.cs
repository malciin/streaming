using Microsoft.AspNetCore.Http;
using Streaming.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Strategies
{
    public class ApiManifestEndpointStrategy : IManifestEndpointStrategy
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public ApiManifestEndpointStrategy(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public string SetEndpoints(Guid VideoId, string ManifestString)
        {
            var getVideoPartEndpoint = UrlHelper.GetHostUrl(httpContextAccessor.HttpContext) + "/Video";
            return ManifestString
                .Replace("[ENDPOINT]", getVideoPartEndpoint)
                .Replace("[ID]", VideoId.ToString());
        }
    }
}
