using Streaming.Domain.Command;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Application.Commands
{
    public class _CommandBus : ICommandBus
    {
        private readonly Func<Type, ICommandHandler> commandHandlersMapping;

        public _CommandBus(Func<Type, ICommandHandler> commandHandlersMapping)
        {
            this.commandHandlersMapping = commandHandlersMapping;
        }

        public async Task SendAsync<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = (ICommandHandler<TCommand>) commandHandlersMapping(typeof(TCommand));
            await handler.Handle(command);
        }
    }
}
