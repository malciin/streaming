using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Live;
using Streaming.Common.Extensions;
using Streaming.Domain.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Application.Commands.Live
{
    public class StartLiveStreamHandler : ICommandHandler<StartLiveStreamCommand>
    {
        private readonly IMessageSignerService messageSignerService;
        private readonly ILiveStreamManager liveManager;

        public StartLiveStreamHandler(ILiveStreamManager liveManager, IMessageSignerService messageSignerService)
        {
            this.messageSignerService = messageSignerService;
            this.liveManager = liveManager;
        }

        public Task HandleAsync(StartLiveStreamCommand command)
        {
            if (!String.Equals(command.App, "live", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("App must be setted to live");
            }

            var bytes = command.StreamKey.ToByteArrayFromBase32String();
            var clientIdentifierBytes = messageSignerService.GetMessage(bytes);
            var clientIdentifier = Encoding.UTF8.GetString(clientIdentifierBytes);

            liveManager.StartNewStream(new NewLiveStreamDTO
            {
                LiveStreamId = command.StreamId,
                ManifestUri = command.ManifestUri,
                User = new UserDetails { }
            });

            return Task.FromResult(0);
        }
    }
}
