using System.Threading.Tasks;

namespace Streaming.Application.Commands
{
    public interface ICommandDispatcher
    {
        Task HandleAsync<T>(T command) where T : ICommand;
    }
}
