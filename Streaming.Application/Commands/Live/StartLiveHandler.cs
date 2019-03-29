using Streaming.Application.Interfaces.Services;
using Streaming.Application.Models;
using Streaming.Common.Extensions;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Application.Commands.Live
{
    public class StartLiveHandler : ICommandHandler<StartLiveCommand>
    {
        public IMessageSignerService messageSignerService;
        private readonly StreamManager streamManager;

        public StartLiveHandler(IMessageSignerService messageSignerService, StreamManager streamManager)
        {
            this.messageSignerService = messageSignerService;
            this.streamManager = streamManager;
        }

        public Task HandleAsync(StartLiveCommand Command)
        {
            var bytes = Command.ClientKey.ToByteArrayFromBase32String();
            var clientIdentifierBytes = messageSignerService.GetMessage(bytes);
            var clientIdentifier = Encoding.UTF8.GetString(clientIdentifierBytes);

            streamManager.StartNewStream(Command.ClientKey);

            return Task.FromResult(0);
        }
    }
}
