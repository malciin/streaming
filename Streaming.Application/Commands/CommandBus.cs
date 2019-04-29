using Autofac;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Streaming.Application.Commands
{
    public class CommandBus : ICommandBus
	{
		private readonly ILifetimeScope lifetimeScope;
		private readonly ConcurrentQueue<dynamic> queue = new ConcurrentQueue<dynamic>();

        private Task worker;
        private readonly object lockObj = new object();
		private bool running = false;
        

		public CommandBus(ILifetimeScope lifetimeScope)
		{
			this.lifetimeScope = lifetimeScope;
		}

		async Task WorkerTask()
		{
			while(!queue.IsEmpty)
			{
                queue.TryDequeue(out var command);
				using (var scope = lifetimeScope.BeginLifetimeScope())
				{
					var dispatcher = scope.Resolve<ICommandDispatcher>();
                    try
                    {
                        await dispatcher.HandleAsync(command);
                    }
                    catch(Exception ex)
                    {
                        scope.Resolve<ILogger<CommandBus>>().LogError(ex, "Processing video failed");
                    }
                }
			}
			lock(lockObj)
			{
				running = false;
			}
		}

		public void Push(ICommand Command)
		{
			queue.Enqueue(Command);
			lock(lockObj)
			{
                if (running)
                    return;
                running = true;
                worker = Task.Factory.StartNew(WorkerTask, TaskCreationOptions.LongRunning);
            }
		}
	}
}
