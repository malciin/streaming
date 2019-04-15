using System;

namespace Streaming.Domain.Models
{
    public class LiveStream
    {
        public Guid LiveStreamId { get; set; }
        public Uri ManifestUrl { get; set; }
        public DateTime Started { get; set; }
        public DateTime Ended { get; set; }
        public UserDetails Owner { get; set; }
    }
}
