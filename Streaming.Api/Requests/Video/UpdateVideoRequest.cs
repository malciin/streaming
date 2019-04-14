using System;

namespace Streaming.Api.Requests.Video
{
    public class UpdateVideoRequest
    {
        public Guid VideoId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
