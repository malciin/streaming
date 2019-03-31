using System;

namespace Streaming.Application.Commands.Live
{
    public class ConnectLiveServerCommand : ICommand
    {
        public string ClientKey { get; set; }
        public string App { get; set; }
        public Guid StreamId { get; set; }
    }
}
