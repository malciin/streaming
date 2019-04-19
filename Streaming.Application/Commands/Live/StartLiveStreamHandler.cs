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
        private readonly IAuth0Client auth0Client;

        public StartLiveStreamHandler(ILiveStreamManager liveManager, IMessageSignerService messageSignerService, IAuth0Client auth0Client)
        {
            this.messageSignerService = messageSignerService;
            this.liveManager = liveManager;
            this.auth0Client = auth0Client;
        }

        public async Task HandleAsync(StartLiveStreamCommand command)
        {
            if (!String.Equals(command.App, "live", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("App must be setted to live");
            }

            var bytes = command.StreamKey.ToByteArrayFromBase32String();
            var clientIdentifierBytes = messageSignerService.GetMessage(bytes);
            var clientIdentifier = Encoding.UTF8.GetString(clientIdentifierBytes);

            var clientInfo = await auth0Client.GetInfoAsync(clientIdentifier);

            await liveManager.StartNewLiveStreamAsync(new NewLiveStreamDTO
            {
                LiveStreamId = command.StreamId,
                ManifestUri = command.ManifestUri,
                User = new UserDetails
                {
                    Email = clientInfo.Email,
                    Nickname = clientInfo.NickName,
                    UserId = clientInfo.UserId
                }
            });
        }
    }
}
