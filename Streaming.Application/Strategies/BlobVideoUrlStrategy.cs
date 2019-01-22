using Streaming.Application.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Strategies
{
    public class BlobVideoUrlStrategy : IVideoUrlStrategy
    {
        private readonly IVideoBlobService videoBlobService;

        public BlobVideoUrlStrategy(IVideoBlobService videoBlobService)
        {
            this.videoBlobService = videoBlobService;
        }

        public string GetVideoUrl(Guid VideoId, int PartNumber)
            => videoBlobService.GetVideoUrl(VideoId, PartNumber);
    }
}
