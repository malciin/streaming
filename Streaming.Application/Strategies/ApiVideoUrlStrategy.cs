using Microsoft.AspNetCore.Http;
using Streaming.Common.Helpers;
using System;

namespace Streaming.Application.Strategies
{
    public class ApiVideoUrlStrategy : IVideoUrlStrategy
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public ApiVideoUrlStrategy(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }
        public string GetVideoUrl(Guid VideoId, int PartNumber)
        {
            return UrlHelper.GetHostUrl(httpContextAccessor.HttpContext) + 
                $"/Video/{VideoId}/{PartNumber}";
        }
    }
}
