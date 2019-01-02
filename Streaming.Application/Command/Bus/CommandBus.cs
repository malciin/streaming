using Autofac;
using Streaming.Application.Command.Commands.Video;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Streaming.Application.Command.Bus
{

    class CommandBus : ICommandBus
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

		public Status GetBusStatus()
		{
			throw new NotImplementedException();
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
                    await dispatcher.HandleAsync(command);
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
                    worker = WorkerTask();
				}
			}
		}
	}
}
