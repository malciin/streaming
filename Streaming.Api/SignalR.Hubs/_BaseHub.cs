using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Streaming.Api.SignalR.Hubs
{
    public class _BaseHub : Hub
    {
        public _BaseHub()
        {
            
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
