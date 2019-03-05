using System;

namespace Streaming.Application.Commands.Video
{
    public class ProcessVideoCommand : ICommand
    {
        public Guid VideoId { get; set; }
    }
}
