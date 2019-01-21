using Microsoft.AspNetCore.Http;
using Streaming.Common.Extensions;
using Streaming.Common.Helpers;
using Streaming.Domain.Models.Core;
using System;
using System.Text.RegularExpressions;

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
            var getVideoPartEndpoint = UrlHelper.GetHostUrl(httpContextAccessor.HttpContext) + $"/Video/{VideoId}/";
            var pattern = VideoManifest.EndpointPlaceholder.Replace("[", "\\[");
            var match = Regex.Match(ManifestString, pattern);
            int partNum = 0;
            while (match.Success)
            {
                ManifestString = ManifestString.Replace(match.Index, match.Length, $"{getVideoPartEndpoint}{partNum++}");
                match = Regex.Match(ManifestString, pattern);
            }
            return ManifestString;
        }
    }
}
