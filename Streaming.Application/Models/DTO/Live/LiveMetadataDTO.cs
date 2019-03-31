using System;

namespace Streaming.Application.Models.DTO.Live
{
    public class LiveMetadataDTO
    {
        public Guid StreamId { get; set; }
        public string ManifestUrl { get; set; }
    }
}
