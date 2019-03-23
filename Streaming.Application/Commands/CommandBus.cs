using Autofac;
using Streaming.Application.Interfaces.Services;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Streaming.Application.Commands
{
    public class CommandBus : ICommandBus
	{
		private readonly ILifetimeScope lifetimeScope;
		private ConcurrentQueue<dynamic> queue = new ConcurrentQueue<dynamic>();

        private Task worker;
        private object lockObj = new object();
		private bool running = false;
        

		public CommandBus(ILifetimeScope lifetimeScope)
		{
			this.lifetimeScope = lifetimeScope;
		}

		async Task WorkerTask()
		{
			while(!queue.IsEmpty)
			{
				dynamic command;
				queue.TryDequeue(out command);
				using (var scope = lifetimeScope.BeginLifetimeScope())
				{
					var dispatcher = scope.Resolve<ICommandDispatcher>();
                    try
                    {
                        await dispatcher.HandleAsync(command);
                    }
                    catch(Exception ex)
                    {
                        scope.Resolve<ILoggerService>().Log(dispatcher, ex);
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
				if (!running)
				{
					running = true;
                    worker = Task.Factory.StartNew(WorkerTask, TaskCreationOptions.LongRunning);
				}
			}
		}
	}
}
