using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Command.Commands.Video
{
    public class DownloadVideo : ICommand
    {
        public Guid VideoId { get; set; }
    }
}
