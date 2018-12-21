using Streaming.Domain.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Commands
{
    public class DownloadVideo : ICommand
    {
        public Guid VideoId { get; set; }
    }
}
