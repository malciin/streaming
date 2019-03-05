namespace Streaming.Application.Commands
{
    public interface ICommandBus
	{
		void Push(ICommand Command);
	}
}
