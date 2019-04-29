using Streaming.Domain.Models;
using System;

namespace Streaming.Application.Models.DTO.Video
{
    public class VideoMetadataDTO
    {
        public Guid VideoId { get; set; }

        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public TimeSpan Length { get; set; }

        public string OwnerNickname { get; set; }
    }
}
