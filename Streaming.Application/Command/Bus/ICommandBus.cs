using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Streaming.Application.Command.Bus
{
	public interface ICommandBus
	{
		Status GetBusStatus();
		void Push(ICommand Command);
	}
}
