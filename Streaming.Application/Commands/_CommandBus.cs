using Streaming.Domain.Command;
using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Commands
{
    public class _CommandBus : ICommandBus
    {
        private readonly Func<Type, ICommandHandler> commandHandlersMapping;

        public _CommandBus(Func<Type, ICommandHandler> commandHandlersMapping)
        {
            this.commandHandlersMapping = commandHandlersMapping;
        }

        public void Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = (ICommandHandler<TCommand>) commandHandlersMapping(typeof(TCommand));
            handler.Handle(command);
        }
    }
}
