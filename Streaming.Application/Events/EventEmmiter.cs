using Autofac;
using Streaming.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Streaming.Application.Events
{
    public class EventEmmiter : IEventEmmiter
    {
        private readonly ILifetimeScope lifetimeScope;
        public EventEmmiter(ILifetimeScope lifetimeScope)
        {
            this.lifetimeScope = lifetimeScope;
        }

        public async Task Emit<T>(T @event) where T : IEvent
        {
            using (var scope  = lifetimeScope.BeginLifetimeScope())
            {
                var receivers = scope.Resolve<IEnumerable<IEventReceiver<T>>>();
                foreach (var receiver in receivers)
                {
                    try
                    {
                        await receiver.Receive(@event);
                    }
                    catch (Exception ex)
                    {
                        scope.Resolve<ILoggerService>().Log(receiver, ex);
                    }
                }
            }
        }
    }
}
