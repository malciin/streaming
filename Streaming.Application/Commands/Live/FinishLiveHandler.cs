using Streaming.Application.Models;
using System.Threading.Tasks;

namespace Streaming.Application.Commands.Live
{
    public class FinishLiveHandler : ICommandHandler<FinishLiveCommand>
    {
        private readonly LiveManager liveManager;

        public FinishLiveHandler(LiveManager liveManager)
        {
            this.liveManager = liveManager;
        }

        public Task HandleAsync(FinishLiveCommand Command)
        {
            liveManager.FinishStream(Command.StreamId);
            return Task.FromResult(0);
        }
    }
}
