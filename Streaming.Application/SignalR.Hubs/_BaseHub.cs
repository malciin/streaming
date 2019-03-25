using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Streaming.Application.Models;

namespace Streaming.Application.SignalR.Hubs
{
    public abstract class _BaseHub : Hub
    {
        public static readonly HubConnectionsInformation Informations = new HubConnectionsInformation();

        public override Task OnConnectedAsync()
        {
            Informations.Push(Context);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            Informations.Pop(Context);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
