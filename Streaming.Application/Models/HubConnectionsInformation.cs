using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;

namespace Streaming.Application.Models
{
    public class HubConnectionsInformation
    {
        private ConcurrentDictionary<string, string> connectionMapper;

        public HubConnectionsInformation()
        {
            connectionMapper = new ConcurrentDictionary<string, string>();
        }

        public string GetConnectionId(string userId)
            => connectionMapper[userId];

        public void Push(HubCallerContext ctx)
        {
            var userId = ctx.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            connectionMapper.TryAdd(userId, ctx.ConnectionId);
        }

        public void Pop(HubCallerContext ctx)
        {
            var userId = ctx.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            connectionMapper.TryRemove(userId, out _);
        }
    }
}
