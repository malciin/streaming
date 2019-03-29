using Microsoft.AspNetCore.Mvc;

namespace Streaming.Application.Commands.Live
{
    public class StartLiveCommand : ICommand
    {
        public string ClientKey { get; set; }
    }
}
