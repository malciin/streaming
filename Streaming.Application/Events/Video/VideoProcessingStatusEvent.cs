using Streaming.Application.Events;
using System;

namespace Streaming.Application.Models.DTO.Video
{
    public class VideoProcessingStatusEvent : IEvent
    {
        public Guid VideoId { get; set; }
        public string UserId { get; set; }
        public string Output { get; set; }
    }
}
