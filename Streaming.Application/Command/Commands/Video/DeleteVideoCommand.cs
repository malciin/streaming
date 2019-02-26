using System;

namespace Streaming.Application.Command.Commands.Video
{
    public class DeleteVideoCommand : ICommand
    {
        public Guid VideoId { get; set; }
    }
}
