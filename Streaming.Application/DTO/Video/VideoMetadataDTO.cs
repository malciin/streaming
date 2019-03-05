using Streaming.Domain.Models;
using System;

namespace Streaming.Application.DTO.Video
{
    public class VideoMetadataDTO
    {
        public Guid VideoId { get; set; }

        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public TimeSpan Length { get; set; }

        public UserDetails Owner { get; set; }
    }
}
