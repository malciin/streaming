using Autofac;
using System.Threading.Tasks;

namespace Streaming.Application.Commands
{
    public class CommandDispatcher : ICommandDispatcher
    {
        private readonly IComponentContext componentContext;
        public CommandDispatcher(IComponentContext componentContext)
        {
            this.componentContext = componentContext;
        }

        public async Task HandleAsync<T>(T command) where T : ICommand
        {
            var handler = componentContext.Resolve<ICommandHandler<T>>();
            await handler.HandleAsync(command);
        }
    }
}
