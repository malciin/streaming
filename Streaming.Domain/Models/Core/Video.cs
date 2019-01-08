using System;

namespace Streaming.Domain.Models.Core
{
    public class Video
    {
        public Guid VideoId { get; set; }

        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? FinishedProcessingDate { get; set; }
        public string Description { get; set; }
        
        public byte[] VideoThumbnail { get; set; }

        public string VideoOriginalName { get; set; }
        public TimeSpan? Length { get; set; }
        public string VideoManifestHLS { get; set; }

        public string ProcessingInfo { get; set; }
    }
}
