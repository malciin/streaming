using System.Threading.Tasks;

namespace Streaming.Application.Command
{
    public interface ICommandDispatcher
    {
        Task HandleAsync<T>(T command) where T : ICommand;
    }
}
