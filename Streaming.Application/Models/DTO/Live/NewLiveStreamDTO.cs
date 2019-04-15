using Streaming.Domain.Models;
using System;

namespace Streaming.Application.Models.DTO.Live
{
    public class NewLiveStreamDTO
    {
        public Guid LiveStreamId { get; set; }
        public UserDetails User { get; set; }
        public Uri ManifestUri { get; set; }
    }
}
