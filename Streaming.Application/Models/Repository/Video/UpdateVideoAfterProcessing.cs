using Streaming.Domain.Enums;
using System;

namespace Streaming.Application.Models.Repository.Video
{
    public class UpdateVideoAfterProcessing
    {
        public Guid VideoId { get; set; }
        public DateTime FinishedProcessingDate { get; set; }
        public string ProcessingInfo { get; set; }
        public string VideoManifestHLS { get; set; }
        public TimeSpan VideoLength { get; set; }
        public VideoState VideoState { get; set; }
    }
}
