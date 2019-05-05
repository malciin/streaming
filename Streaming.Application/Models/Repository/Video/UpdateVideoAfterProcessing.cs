using Streaming.Domain.Enums;
using Streaming.Domain.Models;
using System;

namespace Streaming.Application.Models.Repository.Video
{
    public class UpdateVideoAfterProcessing
    {
        public Guid VideoId { get; set; }
        public DateTime FinishedProcessingDate { get; set; }
        public string ProcessingInfo { get; set; }
        public VideoManifest VideoManifest { get; set; }
        public TimeSpan VideoLength { get; set; }
        public VideoState VideoState { get; set; }
        public string MainThumbnailUrl { get; set; }
    }
}
