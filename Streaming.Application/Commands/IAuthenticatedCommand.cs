using Streaming.Application.Models;

namespace Streaming.Application.Commands
{
    public interface IAuthenticatedCommand : ICommand
    {
        UserInfo User { get; set; }
    }
}