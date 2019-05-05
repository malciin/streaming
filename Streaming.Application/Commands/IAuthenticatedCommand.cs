using Streaming.Application.Models;
using Streaming.Domain.Models;

namespace Streaming.Application.Commands
{
    public interface IAuthenticatedCommand : ICommand
    {
        UserInfo User { get; set; }
    }
}