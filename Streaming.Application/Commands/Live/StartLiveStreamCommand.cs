using System;

namespace Streaming.Application.Commands.Live
{
    public class StartLiveStreamCommand : ICommand
    {
        public string StreamKey { get; set; }
        public string App { get; set; }
        public Guid StreamId { get; set; }
        public Uri ManifestUri { get; set; }
    }
}
