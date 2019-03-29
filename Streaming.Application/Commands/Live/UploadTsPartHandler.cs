using Streaming.Application.Models;
using System.Threading.Tasks;

namespace Streaming.Application.Commands.Live
{
    public class UploadTsPartHandler : ICommandHandler<UploadTsPartCommand>
    {
        private readonly StreamManager streamManager;

        public UploadTsPartHandler(StreamManager streamManager)
        {
            this.streamManager = streamManager;
        }

        public Task HandleAsync(UploadTsPartCommand Command)
        {
            streamManager.Upload(Command.StreamKey, Command.Part, Command.FileManifestDetails);
            return Task.FromResult(0);
        }
    }
}
