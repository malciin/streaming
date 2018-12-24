using Autofac;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Streaming.Application.Command.Bus
{
	class CommandBus : ICommandBus
	{
		private readonly ILifetimeScope lifetimeScope;
		private ConcurrentQueue<ICommand> queue = new ConcurrentQueue<ICommand>();
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

		async Task Start()
		{
			while(!queue.IsEmpty)
			{
				ICommand command;
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
					_ = Start();
				}
			}
		}
	}
}
