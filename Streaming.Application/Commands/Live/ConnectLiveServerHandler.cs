using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models;
using Streaming.Common.Extensions;
using Streaming.Domain.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Application.Commands.Live
{
    public class ConnectLiveServerHandler : ICommandHandler<ConnectLiveServerCommand>
    {
        public IMessageSignerService messageSignerService;
        public LiveManager liveManager;

        public ConnectLiveServerHandler(LiveManager liveManager, IMessageSignerService messageSignerService)
        {
            this.messageSignerService = messageSignerService;
            this.liveManager = liveManager;
        }

        public Task HandleAsync(ConnectLiveServerCommand Command)
        {
            if (!String.Equals(Command.App, "live", StringComparison.InvariantCultureIgnoreCase))
            {
                throw new ArgumentException("App must be setted to live");
            }

            var bytes = Command.ClientKey.ToByteArrayFromBase32String();
            var clientIdentifierBytes = messageSignerService.GetMessage(bytes);
            var clientIdentifier = Encoding.UTF8.GetString(clientIdentifierBytes);

            liveManager.StartNewStream(Command.StreamId, new UserDetails
            {
                UserId = clientIdentifier
            });

            return Task.FromResult(0);
        }
    }
}
