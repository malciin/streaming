using System;

namespace Streaming.Application.Commands.Live
{
    public class FinishLiveStreamCommand : ICommand
    {
        public Guid StreamId { get; set; }
    }
}
