using System;

namespace Streaming.Application.Command.Commands.Video
{
    public class ProcessVideoCommand : ICommand
    {
        public Guid VideoId { get; set; }
        public string VideoPath { get; set; }
    }
}
