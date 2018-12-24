using System;
using System.Collections.Generic;
using System.Text;

namespace Streaming.Application.Command.Bus
{
	public class Status
	{
		public int EnqueuedJobs { get; }
		public int RunningJobs { get; }
	}
}
