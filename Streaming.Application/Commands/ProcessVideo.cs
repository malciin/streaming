using Streaming.Domain.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Commands
{
    public class ProcessVideo : ICommand
    {
        public Guid VideoId { get; set; }
        public string RawVideoLocalPath { get; set; }
    }
}
