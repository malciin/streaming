using System;

namespace Streaming.Application.Command.Commands.Video
{
    public class ProcessVideo : ICommand
    {
        public Guid VideoId { get; set; }
        public string VideoPath { get; set; }
    }
}
