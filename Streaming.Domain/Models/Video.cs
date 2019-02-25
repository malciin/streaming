using Streaming.Domain.Enums;
using System;

namespace Streaming.Domain.Models
{
    public class Video
    {
        public Guid VideoId { get; set; }
        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? FinishedProcessingDate { get; set; }
        public string Description { get; set; }
        public VideoState State { get; set; }

        public TimeSpan? Length { get; set; }
        public string VideoManifestHLS { get; set; }
        public string ProcessingInfo { get; set; }
    }
}
