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

        public async Task HandleAsync(FinishLiveStreamCommand command)
        {
            await liveManager.FinishLiveStreamAsync(command.StreamId);
        }
    }
}
