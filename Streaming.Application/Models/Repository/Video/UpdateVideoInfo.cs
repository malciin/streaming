using System;

namespace Streaming.Application.Models.Repository.Video
{
    public class UpdateVideoInfo
    {
        public Guid UpdateByVideoId { get; set; }
        public string UpdateByUserIdentifier { get; set; }
        public string NewVideoTitle { get; set; }
        public string NewVideoDescription { get; set; }
    }
}