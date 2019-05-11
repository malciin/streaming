using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models.DTO.Live;
using Streaming.Common.Extensions;
using System;
using System.Text;
using System.Threading.Tasks;
using Streaming.Application.Interfaces.Repositories;

namespace Streaming.Application.Commands.Live
{
    public class StartLiveStreamHandler : ICommandHandler<StartLiveStreamCommand>
    {
        private readonly IMessageSignerService messageSignerService;
        private readonly ILiveStreamManager liveManager;
        private readonly IUserRepository userRepository;

        public StartLiveStreamHandler(ILiveStreamManager liveManager, IMessageSignerService messageSignerService, IUserRepository userRepository)
        {
            this.messageSignerService = messageSignerService;
            this.userRepository = userRepository;
            this.liveManager = liveManager;
        }

        public async Task HandleAsync(StartLiveStreamCommand command)
        {
            if (!String.Equals(command.App, "live", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("RTMP app path must be equal 'live'");
            }

            var bytes = command.StreamKey.ToByteArrayFromBase32String();
            var clientIdentifierBytes = messageSignerService.GetMessage(bytes);
            var clientIdentifier = Encoding.UTF8.GetString(clientIdentifierBytes);

            var clientInfo = await userRepository.GetSingleAsync(clientIdentifier);
            await liveManager.StartNewLiveStreamAsync(new NewLiveStreamDTO
            {
                LiveStreamId = command.StreamId,
                ManifestUri = command.ManifestUri,
                User = clientInfo
            });
        }
    }
}
