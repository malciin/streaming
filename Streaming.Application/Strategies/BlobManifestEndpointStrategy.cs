using Streaming.Application.Repository;
using Streaming.Domain.Models.Core;
using System;
using System.Text.RegularExpressions;
using Streaming.Common.Extensions;

namespace Streaming.Application.Strategies
{
    public class BlobManifestEndpointStrategy : IManifestEndpointStrategy
    {
        private readonly IVideoBlobRepository blobRepository;

        public BlobManifestEndpointStrategy(IVideoBlobRepository blobRepository)
        {
            this.blobRepository = blobRepository;
        }

        public string SetEndpoints(Guid VideoId, string ManifestString)
        {
            var pattern = VideoManifest.EndpointPlaceholder.Replace("[", "\\[");
            var match = Regex.Match(ManifestString, pattern);
            int partNum = 0;
            while (match.Success)
            {
                ManifestString = ManifestString.Replace(match.Index, match.Length, blobRepository.GetVideoUrl(VideoId, partNum++));
                match = Regex.Match(ManifestString, pattern);
            }
            return ManifestString;
        }
    }
}
