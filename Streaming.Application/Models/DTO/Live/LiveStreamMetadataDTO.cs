using System;

namespace Streaming.Application.Models.DTO.Live
{
    public class LiveStreamMetadataDTO
    {
        public Guid LiveStreamId { get; set; }
        public string ManifestUrl { get; set; }
    }
}
