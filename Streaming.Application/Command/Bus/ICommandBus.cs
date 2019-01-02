namespace Streaming.Application.Command.Bus
{
    public interface ICommandBus
	{
		Status GetBusStatus();
		void Push(ICommand Command);
	}
}
