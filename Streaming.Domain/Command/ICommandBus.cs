using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Domain.Command
{
    public interface ICommandBus
    {
        void Send<TCommand>(TCommand command) where TCommand : ICommand;
    }
}
