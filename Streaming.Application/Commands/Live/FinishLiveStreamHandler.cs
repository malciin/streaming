using Streaming.Application.Interfaces.Services;
using System.Threading.Tasks;

namespace Streaming.Application.Commands.Live
{
    public class FinishLiveStreamHandler : ICommandHandler<FinishLiveStreamCommand>
    {
        private readonly ILiveStreamManager liveManager;

        public FinishLiveStreamHandler(ILiveStreamManager liveManager)
        {
            this.liveManager = liveManager;
        }

        public Task HandleAsync(FinishLiveStreamCommand command)
        {
            liveManager.FinishStream(command.StreamId);
            return Task.FromResult(0);
        }
    }
}
