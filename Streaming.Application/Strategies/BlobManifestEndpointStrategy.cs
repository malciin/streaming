using Streaming.Domain.Models.Core;
using System;
using System.Text.RegularExpressions;
using Streaming.Common.Extensions;
using Streaming.Application.Services;

namespace Streaming.Application.Strategies
{
    public class BlobManifestEndpointStrategy : IManifestEndpointStrategy
    {
        private readonly IVideoBlobService videoBlobService;

        public BlobManifestEndpointStrategy(IVideoBlobService videoBlobService)
        {
            this.videoBlobService = videoBlobService;
        }

        public string SetEndpoints(Guid VideoId, string ManifestString)
        {
            var pattern = VideoManifest.EndpointPlaceholder.Replace("[", "\\[");
            var match = Regex.Match(ManifestString, pattern);
            int partNum = 0;
            while (match.Success)
            {
                ManifestString = ManifestString.Replace(match.Index, match.Length, videoBlobService.GetVideoUrl(VideoId, partNum++));
                match = Regex.Match(ManifestString, pattern);
            }
            return ManifestString;
        }
    }
}
