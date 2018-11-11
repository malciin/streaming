using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Domain.Models.DTO
{
    public class VideoProcessedDataDTO
    {
        public Guid VideoId { get; set; }
        public TimeSpan Length { get; set; }
        public string VideoManifestHLS { get; set; }
        public string ProcessingInfo { get; set; }
        public byte[] VideoSegmentsZip { get; set; }
        public DateTime FinishedProcessingDate { get; set; }
    }
}
