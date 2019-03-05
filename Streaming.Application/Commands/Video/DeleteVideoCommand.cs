using System;

namespace Streaming.Application.Commands.Video
{
    public class DeleteVideoCommand : ICommand
    {
        public Guid VideoId { get; set; }
    }
}
