using Streaming.Application.Models;
using System.Threading.Tasks;

namespace Streaming.Application.Commands.Live
{
    public class StartLiveHandler : ICommandHandler<StartLiveCommand>
    {
        private readonly LiveManager liveManager;

        public StartLiveHandler(LiveManager liveManager)
        {
            this.liveManager = liveManager;
        }

        public Task HandleAsync(StartLiveCommand Command)
        {
            liveManager.AddManifestUrl(Command.StreamId, Command.ManifestUrl);
            return Task.FromResult(0);  
        }
    }
}
