using System;

namespace Streaming.Application.Models.DTO.Live
{
    public class LiveStreamMetadataDTO
    {
        public string Title { get; set; }
        public Guid LiveStreamId { get; set; }
        public string ManifestUrl { get; set; }
        public DateTime Started { get; set; }
        public string UserStarted { get; set; }
    }
}
