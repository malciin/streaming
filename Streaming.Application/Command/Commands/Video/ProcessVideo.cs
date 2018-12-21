using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Command.Commands.Video
{
    public class ProcessVideo : ICommand
    {
        public Guid VideoId { get; set; }
        public IFormFile Video { get; set; }
    }
}
