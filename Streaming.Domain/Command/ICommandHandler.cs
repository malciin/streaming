using System.Threading.Tasks;

namespace Streaming.Domain.Command
{
    public interface ICommandHandler { }
    public interface ICommandHandler<TCommand> : ICommandHandler where TCommand : ICommand
    {
        Task Handle(TCommand Command);
    }
}
