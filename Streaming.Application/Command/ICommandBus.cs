using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Application.Command
{
    public interface ICommandBus
    {
        Task HandleAsync<T>(T command) where T : ICommand;
    }
}
