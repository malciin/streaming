using System;

namespace Streaming.Application.Commands.Live
{
    public class FinishLiveCommand : ICommand
    {
        public Guid StreamId { get; set; }
    }
}
