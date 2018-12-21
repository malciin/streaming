using System.Threading.Tasks;

namespace Streaming.Application.Command
{
    public interface ICommandHandler<TCommand> where TCommand : ICommand
    {
        Task HandleAsync(TCommand Command);
    }
}
