using Autofac;
using System.Threading.Tasks;

namespace Streaming.Application.Command
{
    public class CommandBus : ICommandBus
    {
        private readonly IComponentContext componentContext;
        public CommandBus(IComponentContext componentContext)
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
