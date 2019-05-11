using Autofac;
using System;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.Extensions.Logging;

namespace Streaming.Application.Commands
{
    public class CommandBus : ICommandBus
    {
	    private readonly ILifetimeScope lifetimeScope;
	    private readonly ActionBlock<ICommand> actionBlock;

	    public CommandBus(ILifetimeScope lifetimeScope)
		{
			this.lifetimeScope = lifetimeScope;
			actionBlock = new ActionBlock<ICommand>(WorkerTask, new ExecutionDataflowBlockOptions
			{
				MaxDegreeOfParallelism = 4
			});
		}

		private async Task WorkerTask(ICommand command)
		{
			using (var scope = lifetimeScope.BeginLifetimeScope())
			{
				var dispatcher = scope.Resolve<ICommandDispatcher>();
                try
                {
                    await dispatcher.HandleAsync((dynamic)command);
                }
                catch(Exception ex)
                {
                    scope.Resolve<ILogger<CommandBus>>().LogError(ex, "Processing video failed");
                }
            }
		}

		public void Push(ICommand command)
			=> actionBlock.Post(command);
	}
}
