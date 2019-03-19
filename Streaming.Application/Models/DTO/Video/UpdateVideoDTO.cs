using System;

namespace Streaming.Application.Models.DTO.Video
{
    public class UpdateVideoDTO
    {
        public Guid VideoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
