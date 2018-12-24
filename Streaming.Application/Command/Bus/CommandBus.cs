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
		private ConcurrentQueue<ICommand> queue = new ConcurrentQueue<ICommand>();
		private object queueLock = new object();

		private bool running = false;

		public Status GetBusStatus()
		{
			throw new NotImplementedException();
		}

		public void Push(ICommand Command)
		{
			lock(queueLock)
			{
				queue.Enqueue(Command);
				if (running == false)
				{
					running = true;
				}
			}
		}
	}
}
