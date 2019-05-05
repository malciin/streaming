using System;

namespace Streaming.Application.Models.DTO.Live
{
    public class PastLiveStreamMetadataDTO : LiveStreamMetadataDTO
    {
        public DateTime Ended { get; set; }
    }
}