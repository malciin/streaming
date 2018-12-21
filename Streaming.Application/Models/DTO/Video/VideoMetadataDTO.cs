using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Domain.Models.DTO.Video
{
    public class VideoBasicMetadataDTO
    {
        public Guid VideoId { get; set; }

        public string Title { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }

        public TimeSpan Length { get; set; }
    }
}
